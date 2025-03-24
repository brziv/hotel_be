using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public ReportController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetBookingData")]
        public async Task<ActionResult> GetBookingData(DateTime start, DateTime end)
        {
            var bookings = await dbc.TblBookings
                .Include(b => b.TblBookingRooms)
                .Where(b => b.TblBookingRooms.Any(r =>
                (r.BrCheckInDate <= end && r.BrCheckOutDate >= start)))
                .Select(b => new
                {
                    checkInTime = b.TblBookingRooms.Any() ? b.TblBookingRooms.Min(r => r.BrCheckInDate) : (DateTime?)null,
                    checkOutTime = b.TblBookingRooms.Any() ? b.TblBookingRooms.Max(r => r.BrCheckOutDate) : (DateTime?)null,
                    totalMoney = b.BTotalMoney,
                    status = b.BBookingStatus
                })
                .ToListAsync();

            return Ok(new { data = bookings });
        }

        [HttpGet]
        [Route("GetRoomData")]
        public async Task<ActionResult> GetRoomData(DateTime start, DateTime end)
        {
            // Validate date range
            if (start > end) return BadRequest("Start date must be before end date");

            // Get total rooms count
            var totalRooms = await dbc.TblRooms.CountAsync();

            // Get occupied rooms count (rooms with check-in before end and check-out after start)
            var occupiedRooms = await dbc.TblBookingRooms
                .Where(br => br.BrCheckInDate <= end && br.BrCheckOutDate >= start)
                .Select(br => br.BrRoomId) // Guid type
                .Distinct()
                .CountAsync();

            // Get booking rooms with their associated paid bookings
            var bookingRoomsWithRevenue = await dbc.TblBookingRooms
                .Join(
                    dbc.TblBookings.Where(b => b.BBookingStatus == "Paid"),
                    br => br.BrBookingId,
                    b => b.BBookingId,
                    (br, b) => new
                    {
                        RoomId = br.BrRoomId, // Guid type
                        Revenue = b.BTotalMoney ?? 0m, // Assuming decimal?
                        CheckInDate = br.BrCheckInDate,
                        CheckOutDate = br.BrCheckOutDate
                    }
                )
                .Where(x => x.CheckInDate <= end && x.CheckOutDate >= start)
                .ToListAsync();

            // Get all room types
            var rooms = await dbc.TblRooms
                .Select(r => new
                {
                    RoomId = r.RRoomId, // Guid type
                    RoomType = r.RRoomType
                })
                .ToListAsync();

            // Calculate revenue by room type
            var roomRevenueDict = new Dictionary<Guid, decimal>();
            foreach (var bookingRoom in bookingRoomsWithRevenue)
            {
                if (roomRevenueDict.ContainsKey(bookingRoom.RoomId))
                    roomRevenueDict[bookingRoom.RoomId] += bookingRoom.Revenue;
                else
                    roomRevenueDict[bookingRoom.RoomId] = bookingRoom.Revenue;
            }

            var roomTypeRevenue = rooms
                .GroupBy(r => r.RoomType)
                .Select(g => new
                {
                    name = g.Key,
                    revenue = g.Sum(r => roomRevenueDict.ContainsKey(r.RoomId) ? roomRevenueDict[r.RoomId] : 0)
                })
                .OrderByDescending(x => x.revenue)
                .ToList();

            return Ok(new { data = new { totalRooms, occupiedRooms, roomTypes = roomTypeRevenue } });
        }

        [HttpGet]
        [Route("GetServiceData")]
        public async Task<ActionResult> GetServiceData(DateTime start, DateTime end)
        {
            // Validate date range
            if (start > end) return BadRequest("Start date must be before end date");

            // Normalize start and end dates to UTC and truncate milliseconds for consistency
            start = start.ToUniversalTime().Date.Add(start.TimeOfDay).AddTicks(-(start.Ticks % TimeSpan.TicksPerSecond));
            end = end.ToUniversalTime().Date.Add(end.TimeOfDay).AddTicks(-(end.Ticks % TimeSpan.TicksPerSecond));

            // Step 1: Check if there are any booking services between start and end
            var bookingServicesCount = await dbc.TblBookingServices
                .CountAsync(bs => bs.BsCreatedAt >= start && bs.BsCreatedAt <= end);
            if (bookingServicesCount == 0)
            {
                var minCreatedAt = await dbc.TblBookingServices
                    .Where(bs => bs.BsCreatedAt != null)
                    .MinAsync(bs => bs.BsCreatedAt);
                var maxCreatedAt = await dbc.TblBookingServices
                    .Where(bs => bs.BsCreatedAt != null)
                    .MaxAsync(bs => bs.BsCreatedAt);
                return Ok(new
                {
                    data = new List<object>(),
                    debug = $"No booking services found with BsCreatedAt between {start} and {end}. Total BookingServices: {bookingServicesCount}, Min BsCreatedAt: {minCreatedAt}, Max BsCreatedAt: {maxCreatedAt}"
                });
            }

            // Step 2: Get booking services with their bookings and services
            var bookingServicesData = await dbc.TblBookingServices
                .Where(bs => bs.BsCreatedAt >= start && bs.BsCreatedAt <= end)
                .Join(
                    dbc.TblBookings,
                    bs => bs.BsBookingId,
                    b => b.BBookingId,
                    (bs, b) => new { BookingService = bs, Booking = b }
                )
                .Join(
                    dbc.TblServices,
                    bs => bs.BookingService.BsServiceId,
                    s => s.SServiceId,
                    (bs, s) => new
                    {
                        ServiceName = s.SServiceName,
                        Quantity = bs.BookingService.BsQuantity,
                        SellPrice = s.SServiceSellPrice,
                        BookingServiceCreatedAt = bs.BookingService.BsCreatedAt
                    }
                )
                .ToListAsync();

            // Step 3: If no data after joins, provide debug info
            if (!bookingServicesData.Any())
            {
                var totalBookingServices = await dbc.TblBookingServices.CountAsync();
                var totalServices = await dbc.TblServices.CountAsync();
                var totalBookings = await dbc.TblBookings.CountAsync();
                return Ok(new
                {
                    data = new List<object>(),
                    debug = $"No matching booking services found after joins. Total BookingServices: {totalBookingServices}, Total Services: {totalServices}, Total Bookings: {totalBookings}"
                });
            }

            // Step 4: Aggregate in memory
            var serviceDict = new Dictionary<string, (decimal revenue, int count)>();
            foreach (var item in bookingServicesData)
            {
                var revenue = (decimal)item.Quantity * item.SellPrice;
                if (serviceDict.ContainsKey(item.ServiceName))
                {
                    serviceDict[item.ServiceName] = (
                        serviceDict[item.ServiceName].revenue + revenue,
                        serviceDict[item.ServiceName].count + item.Quantity
                    );
                }
                else
                {
                    serviceDict[item.ServiceName] = (revenue, item.Quantity);
                }
            }

            var services = serviceDict
                .Select(kvp => new
                {
                    name = kvp.Key,
                    revenue = kvp.Value.revenue,
                    count = kvp.Value.count
                })
                .OrderByDescending(x => x.revenue)
                .ToList();

            return Ok(new { data = services });
        }

        [HttpGet]
        [Route("GetCostData")]
        public async Task<ActionResult> GetCostData(DateTime start, DateTime end)
        {
            // Validate date range
            if (start > end) return BadRequest("Start date must be before end date");

            var goodsCosts = await dbc.TblImportGoods
                .Where(ig => ig.IgImportDate >= start && ig.IgImportDate <= end)
                .Select(ig => new
                {
                    type = "Goods",
                    amount = ig.IgSumPrice, // Assuming non-nullable decimal
                    date = ig.IgImportDate
                })
                .ToListAsync();

            var totalAmount = goodsCosts.Sum(c => c.amount);

            return Ok(new
            {
                data = new
                {
                    costs = goodsCosts,
                    totalAmount
                }
            });
        }

        [HttpGet]
        [Route("GetSummary")]
        public async Task<ActionResult> GetSummary(DateTime start, DateTime end)
        {
            // Validate date range
            if (start > end) return BadRequest("Start date must be before end date");

            // Total bookings
            var totalBookings = await dbc.TblBookings
                .Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end)
                .CountAsync();

            // Room revenue (from paid bookings)
            var roomRevenue = await dbc.TblBookings
                .Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end && b.BBookingStatus == "Paid")
                .SumAsync(b => b.BTotalMoney ?? 0m);

            // Service revenue
            var serviceRevenue = await dbc.TblBookingServices
                .Join(
                    dbc.TblBookings.Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end),
                    bs => bs.BsBookingId,
                    b => b.BBookingId,
                    (bs, b) => bs
                )
                .Join(
                    dbc.TblServices,
                    bs => bs.BsServiceId,
                    s => s.SServiceId,
                    (bs, s) => (decimal)bs.BsQuantity * s.SServiceSellPrice
                )
                .SumAsync();

            // Total costs
            var totalCosts = await dbc.TblImportGoods
                .Where(ig => ig.IgImportDate >= start && ig.IgImportDate <= end)
                .SumAsync(ig => ig.IgSumPrice);

            // Bookings by status
            var bookingsByStatus = await dbc.TblBookings
                .Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end)
                .GroupBy(b => b.BBookingStatus)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            // Occupancy rate (using the provided start and end)
            var totalRooms = await dbc.TblRooms.CountAsync();
            var totalDays = (end - start).Days + 1;
            var totalRoomDays = totalRooms * totalDays;

            var bookingRooms = await dbc.TblBookingRooms
                .Where(br => br.BrCheckInDate <= end && br.BrCheckOutDate >= start)
                .ToListAsync();
            var occupiedRoomDays = 0;
            foreach (var br in bookingRooms)
            {
                var overlapStart = br.BrCheckInDate > start ? br.BrCheckInDate : start;
                var overlapEnd = br.BrCheckOutDate < end ? br.BrCheckOutDate : end;
                var days = (overlapEnd - overlapStart).Days + 1;
                if (days > 0) occupiedRoomDays += days;
            }
            var occupancyRate = totalRoomDays > 0 ? (decimal)occupiedRoomDays / totalRoomDays * 100 : 0;

            var summary = new
            {
                totalBookings,
                roomRevenue,
                serviceRevenue,
                totalRevenue = roomRevenue + serviceRevenue,
                totalCosts,
                netProfit = roomRevenue + serviceRevenue - totalCosts,
                occupancyRate = Math.Round(occupancyRate, 2),
                bookingsByStatus
            };

            return Ok(new { data = summary });
        }
    }
}