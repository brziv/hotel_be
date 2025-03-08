using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        DBCnhom4 dbc;
        public BookingController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetBookingList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblBookings.ToList() });
        }

        [HttpGet]
        [Route("SearchTblBooking")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblBookings
                .Where(item =>
                    item.BBookingId.ToString().Contains(s) ||
                    item.BGuestId.ToString().Contains(s) ||
                    item.BCheckInDate.ToString().Contains(s) ||
                    item.BCheckOutDate.ToString().Contains(s) ||
                    item.BBookingStatus.Contains(s) ||
                    (item.BTotalMoney.HasValue && item.BTotalMoney.Value.ToString().Contains(s)) ||
                    (item.BDeposit.HasValue && item.BDeposit.Value.ToString().Contains(s)) ||
                    (item.BCreatedAt.HasValue && item.BCreatedAt.Value.ToString().Contains(s))
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblBooking")]
        public ActionResult Them(TblBooking booking)
        {
            dbc.TblBookings.Add(booking);
            dbc.SaveChanges();

            return Ok(new { data = booking });
        }

        [HttpPost]
        [Route("UpdateTblBooking")]
        public ActionResult Sua(Guid bBookingId, Guid bGuestId, DateTime bCheckInDate, DateTime bCheckOutDate, string bBookingStatus, decimal? bTotalMoney, decimal? bDeposit, DateTime? bCreatedAt)
        {
            TblBooking booking = new TblBooking
            {
                BGuestId = bGuestId,
                BCheckInDate = bCheckInDate,
                BCheckOutDate = bCheckOutDate,
                BBookingStatus = bBookingStatus,
                BTotalMoney = bTotalMoney,
                BDeposit = bDeposit,
                BCreatedAt = bCreatedAt
            };
            dbc.SaveChanges();

            return Ok(new { data = booking });
        }

        [HttpPost]
        [Route("DeleteTblBooking")]
        public ActionResult Xoa(Guid bBookingId)
        {
            TblBooking booking = new TblBooking
            {
                BBookingId = bBookingId
            };

            dbc.TblBookings.Remove(booking);
            dbc.SaveChanges();
            return Ok(new { data = booking });
        }


        [HttpPost]
        [Route("Checkin")]
        public IActionResult Checkin(Guid id)
        {

            dbc.Database.ExecuteSqlRaw("EXEC pro_check_in {0}", id);

            return NoContent();
        }
        [HttpPost]
        [Route("Checkout")]
        public IActionResult Checkout(Guid id, String paymethod)
        {

            dbc.Database.ExecuteSqlRaw("EXEC pro_check_out {0}, {1}", id, paymethod);

            return NoContent();
        }

        public class ServiceDto
        {
            public Guid ServiceID { get; set; }
            public int Quantity { get; set; }
        }
        [HttpPost]
        [Route("AddService")]
        public IActionResult AddService(Guid BookingID, [FromBody] List<ServiceDto> services)
        {
            foreach (var service in services)
            {
                dbc.Database.ExecuteSqlRaw("EXEC pro_edit_services {0}, {1}, {2}",
                                            BookingID, service.ServiceID, service.Quantity);
            }

            return NoContent();
        }

    }
}
