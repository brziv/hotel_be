using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DBCnhom4 dbc;

        // Constructor
        public BookingController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        // DTO Classes
        public class BookingRoomsDTO
        {
            public Guid RoomId { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
        }

        public class BookingRequestDTO
        {
            public Guid GuestId { get; set; }
            public List<BookingRoomsDTO>? BRdto { get; set; }
        }

        public class BookingInAdvanceRequestDTO
        {
            public Guid GuestId { get; set; }
            public decimal Deposit { get; set; }
            public List<BookingRoomsDTO>? BRdto { get; set; }
        }

        public class ServiceDto
        {
            public Guid ServiceID { get; set; }
            public int Quantity { get; set; }
        }

        // GET Methods
        [HttpGet]
        [Route("GetBookingList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblBookings.ToList() });
        }

        [HttpGet]
        [Route("SearchTblBooking")]
        public ActionResult Search(string s)
        {
            var results = dbc.TblBookings
                .Where(item =>
                    item.BBookingStatus.Contains(s) ||
                    (item.BTotalMoney.HasValue && item.BTotalMoney.Value.ToString().Contains(s)) ||
                    (item.BDeposit.HasValue && item.BDeposit.Value.ToString().Contains(s)) ||
                    (item.BCreatedAt.HasValue && item.BCreatedAt.Value.ToString().Contains(s)))
                .ToList();

            return Ok(new { data = results });
        }

        [HttpGet]
        [Route("FindAvailableRooms")]
        public IActionResult FindAvailableRooms(DateTime indate, DateTime outdate, string floor)
        {
            using (var cmd = dbc.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    WITH BookedRooms AS (
                        SELECT DISTINCT br.br_RoomID
                        FROM tbl_Bookings b
                        JOIN tbl_BookingRooms br ON b.b_BookingID = br.br_BookingID
                        WHERE (@CheckInDate < DATEADD(HOUR, 1, br.br_CheckOutDate) 
                            AND @CheckOutDate > br.br_CheckInDate)
                    )
                    SELECT r.r_RoomID, r.r_RoomNumber, f.f_Floor, r.r_RoomType, r.r_PricePerHour
                    FROM tbl_Rooms r
                    JOIN tbl_Floors f ON r.r_FloorID = f.f_FloorID
                    WHERE r.r_RoomID NOT IN (SELECT br_RoomID FROM BookedRooms) 
                    AND f.f_Floor = @floor";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddRange(new[]
                {
                    new SqlParameter("@CheckInDate", indate),
                    new SqlParameter("@CheckOutDate", outdate),
                    new SqlParameter("@floor", floor)
                });

                dbc.Database.OpenConnection();
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);

                    var rooms = dt.AsEnumerable().Select(row => new
                    {
                        RoomId = row["r_RoomID"],
                        RoomNumber = row["r_RoomNumber"],
                        Floor = row["f_Floor"],
                        RoomType = row["r_RoomType"],
                        PricePerHour = row["r_PricePerHour"]
                    }).ToList();

                    return Ok(rooms);
                }
            }
        }

        [HttpGet]
        [Route("FindBookings")]
        public IActionResult FindBookings(DateTime indate, DateTime outdate, string floornum)
        {
            using (var cmd = dbc.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT DISTINCT b.*, br.*
                    FROM tbl_Bookings b
                    JOIN tbl_BookingRooms br ON b.b_BookingID = br.br_BookingID
                    JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                    JOIN tbl_Floors f ON r.r_FloorID = f.f_FloorID
                    WHERE (@CheckInDate < br.br_CheckOutDate 
                        AND @CheckOutDate > br.br_CheckInDate)
                    AND f.f_Floor = @Floor";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddRange(new[]
                {
                    new SqlParameter("@CheckInDate", indate),
                    new SqlParameter("@CheckOutDate", outdate),
                    new SqlParameter("@Floor", floornum)
                });

                dbc.Database.OpenConnection();
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);

                    var bookings = dt.AsEnumerable().Select(row => new
                    {
                        BookingId = row["b_BookingID"],
                        GuestId = row["b_GuestID"],
                        CheckInDate = row["br_CheckInDate"],
                        CheckOutDate = row["br_CheckOutDate"],
                        BookingStatus = row["b_BookingStatus"],
                        TotalMoney = row["b_TotalMoney"],
                        Deposit = row["b_Deposit"],
                        CreatedAt = row["b_CreatedAt"]
                    }).ToList();

                    return Ok(bookings);
                }
            }
        }

        // POST Methods
        [HttpPost]
        [Route("InsertTblBooking")]
        public ActionResult Insert(TblBooking booking)
        {
            dbc.TblBookings.Add(booking);
            dbc.SaveChanges();
            return Ok(new { data = booking });
        }

        [HttpPost]
        [Route("UpdateTblBooking")]
        public ActionResult Update(Guid bBookingId, [FromBody] TblBooking booking)
        {
            var existingBooking = dbc.TblBookings.Find(bBookingId);
            if (existingBooking == null)
                return NotFound();

            existingBooking.BGuestId = booking.BGuestId;
            existingBooking.BBookingStatus = booking.BBookingStatus;
            existingBooking.BTotalMoney = booking.BTotalMoney;
            existingBooking.BDeposit = booking.BDeposit;
            existingBooking.BCreatedAt = booking.BCreatedAt;

            dbc.SaveChanges();
            return Ok(new { data = existingBooking });
        }

        [HttpPost]
        [Route("DeleteTblBooking")]
        public ActionResult Delete(Guid bBookingId)
        {
            var booking = dbc.TblBookings.Find(bBookingId);
            if (booking == null)
                return NotFound();

            dbc.TblBookings.Remove(booking);
            dbc.SaveChanges();
            return Ok(new { data = booking });
        }

        [HttpPost]
        [Route("AddGuest")]
        public IActionResult AddGuest(string firstname, string lastname, string email, string phonenum)
        {
            var gg = new TblGuest
            {
                GGuestId = Guid.NewGuid(),
                GFirstName = firstname,
                GLastName = lastname,
                GEmail = email,
                GPhoneNumber = phonenum
            };

            dbc.TblGuests.Add(gg);
            dbc.SaveChanges();
            return Ok(new { data = gg.GGuestId });
        }

        [HttpPost]
        [Route("BookImmediately")]
        public IActionResult BookRoomImmediately([FromBody] BookingRequestDTO request)
        {
            if (request?.BRdto?.Any() != true)
                return BadRequest(new { error = "Invalid data or empty room list" });

            using (var transaction = dbc.Database.BeginTransaction())
            {
                try
                {
                    var bb = new TblBooking
                    {
                        BBookingId = Guid.NewGuid(),
                        BGuestId = request.GuestId,
                        BBookingStatus = "Confirmed",
                        BTotalMoney = 0,
                        BDeposit = 0,
                        BCreatedAt = DateTime.Now
                    };

                    dbc.TblBookings.Add(bb);

                    decimal totalMoney = 0;
                    var roomIds = request.BRdto.Select(r => r.RoomId).ToList();
                    var rooms = dbc.TblRooms.Where(r => roomIds.Contains(r.RRoomId)).ToList();

                    if (rooms.Count != roomIds.Count)
                    {
                        transaction.Rollback();
                        return BadRequest(new { error = "One or more rooms don't exist" });
                    }

                    foreach (var roomDto in request.BRdto)
                    {
                        var room = rooms.First(r => r.RRoomId == roomDto.RoomId);
                        double totalHours = Math.Ceiling((roomDto.CheckOutDate - roomDto.CheckInDate).TotalHours);
                        if (totalHours <= 0)
                        {
                            transaction.Rollback();
                            return BadRequest(new { error = "Check-out date must be after check-in date" });
                        }
                        decimal roomCost = (decimal)totalHours * room.RPricePerHour;
                        totalMoney += roomCost;

                        var br = new TblBookingRoom
                        {
                            BrBookingRoomsId = Guid.NewGuid(),
                            BrBookingId = bb.BBookingId,
                            BrRoomId = roomDto.RoomId,
                            BrCheckInDate = roomDto.CheckInDate,
                            BrCheckOutDate = roomDto.CheckOutDate
                        };
                        dbc.TblBookingRooms.Add(br);

                        room.RStatus = "Occupied";
                    }

                    bb.BTotalMoney = totalMoney;
                    dbc.SaveChanges();
                    transaction.Commit();

                    return Ok(new { message = "Booking successful!", bookingId = bb.BBookingId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, new { error = $"Server error: {ex.Message}" });
                }
            }
        }

        [HttpPost]
        [Route("BookInAdvance")]
        public IActionResult BookInAdvance([FromBody] BookingInAdvanceRequestDTO request)
        {
            if (request?.BRdto?.Any() != true)
                return BadRequest(new { error = "Invalid input data" });

            using (var transaction = dbc.Database.BeginTransaction())
            {
                var bb = new TblBooking
                {
                    BBookingId = Guid.NewGuid(),
                    BGuestId = request.GuestId,
                    BBookingStatus = "Pending",
                    BTotalMoney = 0,
                    BDeposit = request.Deposit,
                    BCreatedAt = DateTime.Now
                };

                dbc.TblBookings.Add(bb);

                decimal totalMoney = 0;
                foreach (var roomDto in request.BRdto)
                {
                    var room = dbc.TblRooms.FirstOrDefault(r => r.RRoomId == roomDto.RoomId);
                    if (room == null)
                    {
                        transaction.Rollback();
                        return BadRequest(new { error = $"Room {roomDto.RoomId} not found" });
                    }

                    double totalHours = Math.Ceiling((roomDto.CheckOutDate - roomDto.CheckInDate).TotalHours);
                    if (totalHours <= 0)
                    {
                        transaction.Rollback();
                        return BadRequest(new { error = "Check-out date must be after check-in date" });
                    }
                    decimal roomCost = (decimal)totalHours * room.RPricePerHour;
                    totalMoney += roomCost;

                    var br = new TblBookingRoom
                    {
                        BrBookingRoomsId = Guid.NewGuid(),
                        BrBookingId = bb.BBookingId,
                        BrRoomId = roomDto.RoomId,
                        BrCheckInDate = roomDto.CheckInDate,
                        BrCheckOutDate = roomDto.CheckOutDate
                    };
                    dbc.TblBookingRooms.Add(br);
                }

                bb.BTotalMoney = totalMoney;
                dbc.SaveChanges();
                transaction.Commit();

                return Ok(new { data = "Advance booking successful" });
            }
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
        public IActionResult Checkout(Guid id, string paymethod)
        {
            dbc.Database.ExecuteSqlRaw("EXEC pro_check_out {0}, {1}", id, paymethod);
            return NoContent();
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