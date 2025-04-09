using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public RoomController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetFloorsWithRooms")]
        public ActionResult GetFloorsWithRooms()
        {
            var result = dbc.TblFloors
                .Select(floor => new
                {
                    fFloorId = floor.FFloorId,
                    fFloor = floor.FFloor,
                    rooms = dbc.TblRooms
                        .Where(room => room.RFloorId == floor.FFloorId)
                        .Select(room => new
                        {
                            roomId = room.RRoomId,
                            roomNumber = room.RRoomNumber,
                            roomType = room.RRoomType,
                            pricePerHour = room.RPricePerHour,
                            status = room.RStatus
                        }).ToList()
                }).ToList();

            return Ok(new { data = result });
        }
        [HttpGet]
        [Route("GetFloorList")]
        public ActionResult GetFloorList()
        {
            return Ok(new { data = dbc.TblFloors.ToList() });
        }

        [HttpPost]
        [Route("AddFloor")]
        public IActionResult AddFloor(string floornum)
        {
            var floor = new TblFloor
            {
                FFloorId = Guid.NewGuid(),
                FFloor = floornum
            };

            dbc.TblFloors.Add(floor);
            dbc.SaveChanges();
            return Ok(new { data = floor.FFloorId });
        }

        [HttpPost]
        [Route("AddRoom")]
        public IActionResult AddRoom(string roomNumber, Guid floorId, string roomType, decimal pricePerHour)
        {
            var room = new TblRoom
            {
                RRoomId = Guid.NewGuid(),
                RRoomNumber = roomNumber,
                RFloorId = floorId,
                RRoomType = roomType,
                RPricePerHour = pricePerHour,
                RStatus = "Available"
            };

            dbc.TblRooms.Add(room);
            dbc.SaveChanges();
            return Ok(new { data = room.RRoomId });
        }

        [HttpDelete]
        [Route("DeleteRoom")]
        public IActionResult DeleteRoom(Guid roomId)
        {
            var room = dbc.TblRooms.FirstOrDefault(r => r.RRoomId == roomId);
            if (room == null)
            {
                return NotFound(new { message = "Room not found." });
            }

            dbc.TblRooms.Remove(room);
            dbc.SaveChanges();
            return Ok(new { message = "Room deleted successfully." });
        }
        [HttpDelete]
        [Route("DeleteFloor")]
        public IActionResult DeleteFloor(Guid floorId)
        {
            var floor = dbc.TblFloors.FirstOrDefault(f => f.FFloorId == floorId);
            if (floor == null)
            {
                return NotFound(new { message = "Floor not found." });
            }

            // Kiểm tra nếu tầng vẫn còn phòng thì không cho xóa
            var hasRooms = dbc.TblRooms.Any(r => r.RFloorId == floorId);
            if (hasRooms)
            {
                return BadRequest(new { message = "Cannot delete floor with existing rooms." });
            }

            dbc.TblFloors.Remove(floor);
            dbc.SaveChanges();
            return Ok(new { message = "Floor deleted successfully." });
        }

        [HttpGet]
        [Route("GetConfirmedBookingsByGuestId")]
        public IActionResult GetBookingsByGuestId(Guid guestId)
        {
            var bookings = dbc.TblBookings
                .Include(b => b.TblBookingRooms)
                    .ThenInclude(br => br.BrRoom)
                .Where(b => b.BGuestId == guestId && b.BBookingStatus == "Confirmed")
                .OrderByDescending(b => b.BCreatedAt) // ⬅️ Sắp xếp giảm dần
                .Select(b => new
                {
                    bookingId = b.BBookingId,
                    status = b.BBookingStatus,
                    totalMoney = b.BTotalMoney,
                    createdAt = b.BCreatedAt,
                    rooms = b.TblBookingRooms.Select(br => new
                    {
                        roomNumber = br.BrRoom.RRoomNumber,
                        checkInDate = br.BrCheckInDate,
                        checkOutDate = br.BrCheckOutDate
                    }).ToList()
                })
                .ToList();

            return Ok(new { data = bookings });
        }
    }
}
