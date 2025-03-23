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
            var totalRooms = await dbc.TblRooms.CountAsync();

            var occupiedRooms = await dbc.TblBookingRooms
                .Where(br => br.BrCheckInDate <= end && br.BrCheckOutDate >= start)
                .Select(br => br.BrRoomId)
                .Distinct()
                .CountAsync();

            var roomTypes = await dbc.TblRooms
                .GroupJoin(
                    dbc.TblBookingRooms
                        .Join(
                            dbc.TblBookings.Where(b => b.BBookingStatus == "Paid"),
                            br => br.BrBookingId,
                            b => b.BBookingId,
                            (br, b) => new { br.BrRoomId, b.BTotalMoney }
                        )
                        .Where(x => x.BTotalMoney != null),
                    room => room.RRoomId,
                    booking => booking.BrRoomId,
                    (room, bookings) => new
                    {
                        name = room.RRoomType,
                        revenue = bookings.Sum(b => b.BTotalMoney) ?? 0
                    })
                .GroupBy(x => x.name)
                .Select(g => new
                {
                    name = g.Key,
                    revenue = g.Sum(x => x.revenue)
                })
                .OrderByDescending(x => x.revenue)
                .ToListAsync();

            return Ok(new { data = new { totalRooms, occupiedRooms, roomTypes } });
        }

        [HttpGet]
        [Route("GetServiceData")]
        public async Task<ActionResult> GetServiceData(DateTime start, DateTime end)
        {
            var services = await dbc.TblServices
                .GroupJoin(
                    dbc.TblBookingServices
                        .Join(
                            dbc.TblBookings.Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end),
                            bs => bs.BsBookingId,
                            b => b.BBookingId,
                            (bs, b) => bs
                        ),
                    service => service.SServiceId,
                    bs => bs.BsServiceId,
                    (service, bookingServices) => new
                    {
                        name = service.SServiceName,
                        revenue = bookingServices.Sum(bs => (decimal)(bs.BsQuantity) * service.SServiceSellPrice),
                        count = bookingServices.Count()
                    })
                .OrderByDescending(x => x.revenue)
                .ToListAsync();

            return Ok(new { data = services });
        }

        [HttpGet]
        [Route("GetCostData")]
        public async Task<ActionResult> GetCostData(DateTime start, DateTime end)
        {
            var goodsCosts = await dbc.TblImportGoods
                .Where(ig => ig.IgImportDate >= start && ig.IgImportDate <= end)
                .Select(ig => new
                {
                    type = "Goods",
                    amount = ig.IgSumPrice,
                    date = ig.IgImportDate
                })
                .ToListAsync();

            var otherCosts = new List<object>();

            var allCosts = goodsCosts.Concat(otherCosts);

            return Ok(new { data = allCosts });
        }

        [HttpGet]
        [Route("GetSummary")]
        public async Task<ActionResult> GetSummary(DateTime start, DateTime end)
        {
            var totalBookings = await dbc.TblBookings
                .Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end)
                .CountAsync();

            var roomRevenue = await dbc.TblBookings
                .Where(b => b.BCreatedAt >= start && b.BCreatedAt <= end && b.BBookingStatus == "Paid")
                .SumAsync(b => b.BTotalMoney ?? 0);

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
                    (bs, s) => (decimal)(bs.BsQuantity) * s.SServiceSellPrice
                )
                .SumAsync();

            var totalCosts = await dbc.TblImportGoods
                .Where(ig => ig.IgImportDate >= start && ig.IgImportDate <= end)
                .SumAsync(ig => ig.IgSumPrice);

            var summary = new
            {
                totalBookings,
                roomRevenue,
                serviceRevenue,
                totalRevenue = roomRevenue + serviceRevenue,
                totalCosts,
                netProfit = roomRevenue + serviceRevenue - totalCosts
            };

            return Ok(new { data = summary });
        }
    }
}