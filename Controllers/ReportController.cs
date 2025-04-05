using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using hotel_be.ModelFromDB;
using hotel_be.DTOs;

[Route("api/[controller]")]
[ApiController]
public class ReportController(DBCnhom4 dbc_in, IConfiguration configuration) : ControllerBase
{
    private readonly DBCnhom4 dbc = dbc_in;
    private readonly string _connectionString = configuration.GetConnectionString("emuach");

    // 1. Booking Reports
    [HttpGet]
    [Route("BookingTotal")]
    public async Task<IActionResult> GetBookingTotal(string interval, DateTime startDate, DateTime endDate)
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
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("BookingStatus")]
    public async Task<IActionResult> GetBookingStatus(string interval, DateTime startDate, DateTime endDate)
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
                AND b.b_BookingStatus != 'Cancel'
            GROUP BY b.b_BookingStatus, DATEPART({datePart}, br.br_CheckInDate)
            ORDER BY Period, b.b_BookingStatus";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<BookingStatusDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("Occupancy")]
    public async Task<IActionResult> GetOccupancyRate(string interval, DateTime startDate, DateTime endDate)
    {
        string datePart = interval.ToLower() switch
        {
            "daily" => "day",
            "weekly" => "week",
            "monthly" => "month",
            _ => throw new ArgumentException("Invalid interval")
        };

        string sql = $@"
            WITH RoomDays AS (
                SELECT 
                    f.f_Floor,
                    DATEDIFF(HOUR, br.br_CheckInDate, br.br_CheckOutDate) / 24.0 AS OccupiedDays,
                    DATEPART({datePart}, br.br_CheckInDate) AS Period
                FROM tbl_BookingRooms br
                JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                JOIN tbl_Floors f ON r.r_FloorID = f.f_FloorID
                WHERE br.br_CheckInDate BETWEEN @startDate AND @endDate
            )
            SELECT 
                f_Floor AS Floor,
                Period,
                SUM(OccupiedDays) AS TotalOccupiedRoomDays,
                (SUM(OccupiedDays) / (9.0 * DATEDIFF(DAY, @startDate, @endDate))) * 100 AS OccupancyRate
            FROM RoomDays
            GROUP BY f_Floor, Period
            ORDER BY f_Floor, Period";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<OccupancyRateDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("RoomTimeline")]
    public async Task<IActionResult> GetRoomTimeline(string interval, DateTime startDate, DateTime endDate)
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
            return Ok(result);
        }
    }

    // 2. Revenue Reports

    [HttpGet]
    [Route("Revenue/Total")]
    public async Task<IActionResult> GetTotalRevenue(string interval, DateTime startDate, DateTime endDate)
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
                SUM(b.b_TotalMoney) AS Amount,
                DATEPART({datePart}, b.b_CreatedAt) AS Period
            FROM tbl_Bookings b
            WHERE b.b_CreatedAt BETWEEN @startDate AND @endDate
                AND b.b_BookingStatus != 'Cancel'
            GROUP BY DATEPART({datePart}, b.b_CreatedAt)
            ORDER BY Period";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("Revenue/Bookings")]
    public async Task<IActionResult> GetBookingRevenue(string interval, DateTime startDate, DateTime endDate)
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
                SUM(r.r_PricePerHour * DATEDIFF(HOUR, br.br_CheckInDate, br.br_CheckOutDate)) AS Amount,
                DATEPART({datePart}, br.br_CheckInDate) AS Period
            FROM tbl_BookingRooms br
            JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
            JOIN tbl_Bookings b ON br.br_BookingID = b.b_BookingID
            WHERE br.br_CheckInDate BETWEEN @startDate AND @endDate
                AND b.b_BookingStatus != 'Cancel'
            GROUP BY DATEPART({datePart}, br.br_CheckInDate)
            ORDER BY Period";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("Revenue/Services")]
    public async Task<IActionResult> GetServiceRevenue(string interval, DateTime startDate, DateTime endDate)
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
            WHERE bs.bs_CreatedAt BETWEEN @startDate AND @endDate
            GROUP BY DATEPART({datePart}, bs.bs_CreatedAt)
            ORDER BY Period";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }

    // 3. Inventory Reports

    [HttpGet]
    [Route("Inventory/Imports")]
    public async Task<IActionResult> GetImportCost(string interval, DateTime startDate, DateTime endDate)
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
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("Inventory/Services")]
    public async Task<IActionResult> GetServiceCost(string interval, DateTime startDate, DateTime endDate)
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
            GROUP BY DATEPART({datePart}, bs.bs_CreatedAt)
            ORDER BY Period";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<RevenueDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("Inventory/Stock")]
    public async Task<IActionResult> GetStockLevels()
    {
        string sql = @"
            SELECT 
                p.p_ProductName AS ProductName,
                p.p_Quantity AS StockLevel,
                p.p_Unit AS Unit
            FROM tbl_Products p
            WHERE p.p_IsService = 0";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<InventoryStockDto>(sql);
            return Ok(result);
        }
    }

    // 4. Trends Reports

    [HttpGet]
    [Route("Trends/RoomTypes")]
    public async Task<IActionResult> GetPopularRoomTypes(string interval, DateTime startDate, DateTime endDate)
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
            return Ok(result);
        }
    }

    [HttpGet]
    [Route("Trends/Services")]
    public async Task<IActionResult> GetPopularServices(string interval, DateTime startDate, DateTime endDate)
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
            GROUP BY sp.sp_PackageName, DATEPART({datePart}, bs.bs_CreatedAt)
            ORDER BY Period, UsageCount DESC";

        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<TrendServiceDto>(sql, new { startDate, endDate });
            return Ok(result);
        }
    }
}