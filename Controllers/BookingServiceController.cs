using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingServiceController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public BookingServiceController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetBookingServiceList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblBookingServices.ToList() });
        }

        [HttpGet]
        [Route("SearchTblBookingService")]
        public ActionResult SearchTblBookingService(string s)
        {
            var results = dbc.TblBookingServices
                .Where(item =>
                    item.BsQuantity.ToString().Contains(s) ||
                    (item.BsCreatedAt.HasValue && item.BsCreatedAt.Value.ToString().Contains(s))
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblBookingService")]
        public ActionResult Them(TblBookingService bookingService)
        {
            dbc.TblBookingServices.Add(bookingService);
            dbc.SaveChanges();

            return Ok(new { data = bookingService });
        }

        [HttpPut]
        [Route("UpdateTblBookingService")]
        public ActionResult Sua(Guid bsBookingServicesId, Guid bsBookingId, Guid bsServiceId, int bsQuantity, DateTime? bsCreatedAt)
        {
            TblBookingService bookingServices = new TblBookingService
            {
                BsBookingServicesId = bsBookingServicesId,
                BsBookingId = bsBookingId,
                BsServiceId = bsServiceId,
                BsQuantity = bsQuantity,
                BsCreatedAt = bsCreatedAt
            };

            dbc.TblBookingServices.Add(bookingServices);
            dbc.SaveChanges();

            return Ok(new { data = bookingServices });
        }


        [HttpDelete]
        [Route("DeleteTblBookingService")]
        public ActionResult Xoa(Guid bsBookingServicesId)
        {
            TblBookingService bookingServices = new TblBookingService
            {
                BsBookingServicesId = bsBookingServicesId,
            };

            dbc.TblBookingServices.Add(bookingServices);
            dbc.SaveChanges();

            return Ok(new { data = bookingServices });
        }
    }
}
