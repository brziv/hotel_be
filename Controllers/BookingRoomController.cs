using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingRoomController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public BookingRoomController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetBookingRoomList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblBookingRooms.ToList() });
        }

        [HttpGet]
        [Route("SearchTblBookingRoom")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblBookingRooms
                .Where(item =>
                    item.BrCheckInDate.ToString().Contains(s) ||
                    item.BrCheckOutDate.ToString().Contains(s)
                )

                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblBookingRoom")]
        public ActionResult Them(TblBookingRoom bookingRoom)
        {
            dbc.TblBookingRooms.Add(bookingRoom);
            dbc.SaveChanges();

            return Ok(new { data = bookingRoom });
        }

        [HttpPut]
        [Route("UpdateTblBookingRoom")]
        public ActionResult Sua(Guid brBookingRoomsId, DateTime brCheckInDate, DateTime brCheckOutDate, Guid brBookingId, Guid brRoomId)
        {
            TblBookingRoom bookingRoom = new TblBookingRoom
            {
                BrBookingRoomsId = brBookingRoomsId,
                BrBookingId = brBookingId,
                BrRoomId = brRoomId,
                BrCheckInDate = brCheckInDate,
                BrCheckOutDate = brCheckOutDate,
            };

            dbc.TblBookingRooms.Add(bookingRoom);
            dbc.SaveChanges();

            return Ok(new { data = bookingRoom });
        }

        [HttpDelete]
        [Route("DeleteTblBookingRoom")]
        public ActionResult Xoa(Guid brBookingRoomsId)
        {
            TblBookingRoom booking = new TblBookingRoom
            {
                BrBookingRoomsId = brBookingRoomsId,
            };

            dbc.TblBookingRooms.Remove(booking);
            dbc.SaveChanges();
            return Ok(new { data = booking });
        }
    }
}
