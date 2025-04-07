using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using hotel_be.ModelFromDB;
using hotel_be.DTOs;
using Microsoft.Extensions.Caching.Memory;

[Route("api/[controller]")]
[ApiController]
public class ReportController(IConfiguration configuration, IMemoryCache cache) : ControllerBase
{
    private readonly string? _connectionString = configuration.GetConnectionString("emuach");
    private readonly IMemoryCache _cache = cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

    // 1. Booking Reports
    [HttpGet]
    [Route("BookingTotal")]
    public async Task<IActionResult> GetBookingTotal(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"BookingTotal_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        // Check if the data is already cached
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<BookingTotalDto>? cachedResult))
        {
            string datePart;
            switch (interval.ToLower())
            {
                case "daily": datePart = "day"; break;
                case "weekly": datePart = "week"; break;
                case "monthly": datePart = "month"; break;
                default: return BadRequest("Invalid interval. Use 'daily', 'weekly', or 'monthly'.");
            }

            string sql = $@"
                SELECT 
                    r.r_RoomNumber AS RoomNumber,
                    COUNT(br.br_ID) AS TotalBookings,
                    DATEPART({datePart}, br.br_CheckInDate) AS Period
                FROM tbl_BookingRooms br
                JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                WHERE br.br_CheckInDate BETWEEN @startDate AND @endDate
                GROUP BY r.r_RoomNumber, DATEPART({datePart}, br.br_CheckInDate)
                ORDER BY r.r_RoomNumber, Period";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<BookingTotalDto>(sql, new { startDate, endDate });
                cachedResult = result;

                // Store the result in cache
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("BookingStatus")]
    public async Task<IActionResult> GetBookingStatus(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"BookingStatus_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<BookingStatusDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
                SELECT 
                    b.b_BookingStatus AS BookingStatus,
                    COUNT(DISTINCT br.br_RoomID) AS RoomCount,
                    DATEPART({datePart}, br.br_CheckInDate) AS Period
                FROM tbl_Bookings b
                JOIN tbl_BookingRooms br ON b.b_BookingID = br.br_BookingID
                WHERE br.br_CheckInDate BETWEEN @startDate AND @endDate
                GROUP BY b.b_BookingStatus, DATEPART({datePart}, br.br_CheckInDate)
                ORDER BY Period, b.b_BookingStatus";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<BookingStatusDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("RoomTimeline")]
    public async Task<IActionResult> GetRoomTimeline(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"RoomTimeline_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<RoomTimelineDto>? cachedResult))
        {
            string sql = @"
                SELECT 
                    r.r_RoomNumber AS RoomNumber,
                    br.br_CheckInDate AS CheckInDate,
                    br.br_CheckOutDate AS CheckOutDate
                FROM tbl_BookingRooms br
                JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                WHERE br.br_CheckInDate BETWEEN @startDate AND @endDate";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RoomTimelineDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    // 2. Revenue Reports
    [HttpGet]
    [Route("Revenue/Total")]
    public async Task<IActionResult> GetTotalRevenue(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"TotalRevenue_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<RevenueDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
            SELECT 
                SUM(ISNULL(roomRevenue, 0) + ISNULL(serviceRevenue, 0) + ISNULL(depositRevenue, 0)) AS Amount,
                DATEPART({datePart}, Period) AS Period
            FROM (
                -- Room Revenue (Paid)
                SELECT 
                    r.r_PricePerHour * DATEDIFF(MINUTE, br.br_CheckInDate, br.br_CheckOutDate) / 60.0 AS roomRevenue,
                    0 AS serviceRevenue,
                    0 AS depositRevenue,
                    br.br_CheckOutDate AS Period  -- Use CheckOutDate for consistency
                FROM tbl_BookingRooms br
                JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                JOIN tbl_Bookings b ON br.br_BookingID = b.b_BookingID
                WHERE br.br_CheckOutDate BETWEEN @startDate AND @endDate  -- Only bookings ending in range
                    AND b.b_BookingStatus = 'Paid'

                UNION ALL
                -- Service Revenue (Confirmed)
                SELECT 
                    0 AS roomRevenue,
                    bs.bs_Quantity * sp.s_ServiceSellPrice AS serviceRevenue,
                    0 AS depositRevenue,
                    bs.bs_CreatedAt AS Period
                FROM tbl_BookingServices bs
                JOIN tbl_ServicePackages sp ON bs.bs_ServiceID = sp.sp_PackageID
                JOIN tbl_Bookings b ON bs.bs_BookingID = b.b_BookingID
                WHERE bs.bs_CreatedAt BETWEEN @startDate AND @endDate
                    AND b.b_BookingStatus = 'Confirmed'
                    AND sp.IsActive = 1

                UNION ALL
                -- Deposit Revenue (Pending and Cancelled)
                SELECT 
                    0 AS roomRevenue,
                    0 AS serviceRevenue,
                    b.b_Deposit AS depositRevenue,
                    br.br_CheckOutDate AS Period
                FROM tbl_Bookings b
                JOIN tbl_BookingRooms br ON b.b_BookingID = br.br_BookingID
                WHERE br.br_CheckOutDate BETWEEN @startDate AND @endDate
                    AND b.b_BookingStatus IN ('Pending', 'Cancelled')
            ) AS RevenueSources
            GROUP BY DATEPART({datePart}, Period)
            ORDER BY Period";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("Revenue/Bookings")]
    public async Task<IActionResult> GetBookingRevenue(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"BookingRevenue_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<RevenueDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
            SELECT 
                SUM(r.r_PricePerHour * DATEDIFF(MINUTE, br.br_CheckInDate, br.br_CheckOutDate) / 60.0) AS Amount,
                DATEPART({datePart}, br.br_CheckOutDate) AS Period  -- Use CheckOutDate
            FROM tbl_BookingRooms br
            JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
            JOIN tbl_Bookings b ON br.br_BookingID = b.b_BookingID
            WHERE br.br_CheckOutDate BETWEEN @startDate AND @endDate  -- Only endings in range
                AND b.b_BookingStatus = 'Paid'
            GROUP BY DATEPART({datePart}, br.br_CheckOutDate)
            ORDER BY Period";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("Revenue/Services")]
    public async Task<IActionResult> GetServiceRevenue(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"ServiceRevenue_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<RevenueDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
            SELECT 
                SUM(bs.bs_Quantity * sp.s_ServiceSellPrice) AS Amount,
                DATEPART({datePart}, bs.bs_CreatedAt) AS Period
            FROM tbl_BookingServices bs
            JOIN tbl_ServicePackages sp ON bs.bs_ServiceID = sp.sp_PackageID
            JOIN tbl_Bookings b ON bs.bs_BookingID = b.b_BookingID
            WHERE bs.bs_CreatedAt BETWEEN @startDate AND @endDate
                AND b.b_BookingStatus = 'Confirmed'
                AND sp.IsActive = 1
            GROUP BY DATEPART({datePart}, bs.bs_CreatedAt)
            ORDER BY Period";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    // 3. Inventory Reports
    [HttpGet]
    [Route("Inventory/Imports")]
    public async Task<IActionResult> GetImportCost(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"ImportCost_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<RevenueDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
                SELECT 
                    SUM(ig.ig_SumPrice) AS Amount,
                    DATEPART({datePart}, ig.ig_ImportDate) AS Period
                FROM tbl_ImportGoods ig
                WHERE ig.ig_ImportDate BETWEEN @startDate AND @endDate
                GROUP BY DATEPART({datePart}, ig.ig_ImportDate)
                ORDER BY Period";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("Inventory/Services")]
    public async Task<IActionResult> GetServiceCost(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"ServiceCost_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<RevenueDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
                SELECT 
                    SUM(bs.bs_Quantity * sp.s_ServiceCostPrice) AS Amount,
                    DATEPART({datePart}, bs.bs_CreatedAt) AS Period
                FROM tbl_BookingServices bs
                JOIN tbl_ServicePackages sp ON bs.bs_ServiceID = sp.sp_PackageID
                WHERE bs.bs_CreatedAt BETWEEN @startDate AND @endDate
                    AND sp.IsActive = 1
                GROUP BY DATEPART({datePart}, bs.bs_CreatedAt)
                ORDER BY Period";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("Inventory/Stock")]
    public async Task<IActionResult> GetStockLevels()
    {
        string cacheKey = "StockLevels";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<InventoryStockDto>? cachedResult))
        {
            string sql = @"
                SELECT 
                    p.p_ProductName AS ProductName,
                    p.p_Quantity AS StockLevel,
                    p.p_Unit AS Unit
                FROM tbl_Products p
                WHERE p.p_IsService = 0
                    AND p.IsActive = 1";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<InventoryStockDto>(sql);
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    // 4. Trends Reports
    [HttpGet]
    [Route("Trends/RoomTypes")]
    public async Task<IActionResult> GetPopularRoomTypes(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"PopularRoomTypes_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<TrendRoomTypeDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
                SELECT 
                    r.r_RoomType AS RoomType,
                    COUNT(br.br_ID) AS BookingCount,
                    DATEPART({datePart}, br.br_CheckInDate) AS Period
                FROM tbl_BookingRooms br
                JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                WHERE br.br_CheckInDate BETWEEN @startDate AND @endDate
                GROUP BY r.r_RoomType, DATEPART({datePart}, br.br_CheckInDate)
                ORDER BY Period, BookingCount DESC";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<TrendRoomTypeDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }

    [HttpGet]
    [Route("Trends/Services")]
    public async Task<IActionResult> GetPopularServices(string interval, DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"PopularServices_{interval}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<TrendServiceDto>? cachedResult))
        {
            string datePart = interval.ToLower() switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => throw new ArgumentException("Invalid interval")
            };

            string sql = $@"
                SELECT 
                    sp.sp_PackageName AS PackageName,
                    SUM(bs.bs_Quantity) AS UsageCount,
                    DATEPART({datePart}, bs.bs_CreatedAt) AS Period
                FROM tbl_BookingServices bs
                JOIN tbl_ServicePackages sp ON bs.bs_ServiceID = sp.sp_PackageID
                WHERE bs.bs_CreatedAt BETWEEN @startDate AND @endDate
                    AND sp.IsActive = 1
                GROUP BY sp.sp_PackageName, DATEPART({datePart}, bs.bs_CreatedAt)
                ORDER BY Period, UsageCount DESC";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<TrendServiceDto>(sql, new { startDate, endDate });
                cachedResult = result;
                _cache.Set(cacheKey, cachedResult, _cacheDuration);
            }
        }
        return Ok(cachedResult);
    }
}