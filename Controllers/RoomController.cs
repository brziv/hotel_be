using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        DBCnhom4 dbc;
        public RoomController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetRoomList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblRooms.ToList() });
        }

        [HttpGet]
        [Route("SearchTblRoom")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblRooms
                .Where(item =>
                    item.RRoomId.ToString().Contains(s) ||
                    item.RRoomNumber.Contains(s) ||
                    item.RFloorId.ToString().Contains(s) ||
                    item.RRoomType.Contains(s) ||
                    item.RPricePerHour.ToString().Contains(s) ||
                    item.RStatus.Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblRoom")]
        public ActionResult Them(TblRoom room)
        {
            dbc.TblRooms.Add(room);
            dbc.SaveChanges();

            return Ok(new { data = room });
        }

        [HttpPost]
        [Route("UpdateTblRoom")]
        public ActionResult Sua(Guid rRoomId, string rRoomNumber, Guid rFloorId, string rRoomType, decimal rPricePerHour, string rStatus)
        {
            TblRoom Room = new TblRoom
            {
                RRoomId = rRoomId,
                RRoomNumber = rRoomNumber,
                RFloorId = rFloorId,
                RRoomType = rRoomType,
                RPricePerHour = rPricePerHour,
                RStatus = rStatus
            };
            dbc.TblRooms.Update(Room);
            dbc.SaveChanges();
            return Ok(new { data = Room });
        }

        [HttpPost]
        [Route("XoaTblRoom")]
        public ActionResult Xoa(Guid rRoomId)
        {
            TblRoom Room = new TblRoom
            {
                RRoomId = rRoomId,
            };

            dbc.TblRooms.Remove(Room);
            dbc.SaveChanges();
            return Ok(new { data = Room });
        }
    }
}
