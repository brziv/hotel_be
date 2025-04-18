﻿using Azure.Core;
using hotel_be.DTOs;
using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using hotel_be.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(DBCnhom4 dbc_in, ICustomCache cache) : ControllerBase
    {
        private readonly DBCnhom4 dbc = dbc_in;
        private readonly ICustomCache _cache = cache;

        // GET Methods
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
	                    WHERE b.b_BookingStatus <> 'Cancelled'
                        AND (@CheckInDate < DATEADD(HOUR, 1, br.br_CheckOutDate) 
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
        [Route("FindAllRooms")]
        public IActionResult FindAllRooms(string floornum)
        {
            using (var cmd = dbc.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"
            SELECT r.r_RoomNumber
            FROM tbl_Rooms r
            JOIN tbl_Floors f ON r.r_FloorID = f.f_FloorID
            WHERE f.f_Floor = @floor
            ORDER BY r.r_RoomNumber";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@floor", floornum));

                dbc.Database.OpenConnection();
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);

                    var roomNumbers = dt.AsEnumerable()
                                        .Select(row => row["r_RoomNumber"].ToString())
                                        .ToList();

                    return Ok(roomNumbers);
                }
            }
        }

        [HttpGet]
        [Route("FindBookings")]
        public IActionResult FindBookings(DateTime indate, DateTime outdate, string floornum)
        {
            string cacheKey = $"bookings_{indate:yyyyMMddHHmm}_{outdate:yyyyMMddHHmm}_{floornum}";

            //if (_cache.TryGetValue(cacheKey, out List<object>? cachedBookings))
            //{
            //    return Ok(new { bookings = cachedBookings });
            //}

            using (var cmd = dbc.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT DISTINCT r.r_RoomNumber,b.b_BookingID,b.b_BookingStatus,b.b_TotalMoney,b.b_Deposit,g.g_FirstName,g.g_LastName,br_CheckInDate,br_CheckOutDate,r.r_PricePerHour
                    FROM tbl_Bookings b
                    JOIN tbl_BookingRooms br ON b.b_BookingID = br.br_BookingID
                    JOIN tbl_Rooms r ON br.br_RoomID = r.r_RoomID
                    JOIN tbl_Floors f ON r.r_FloorID = f.f_FloorID
                    JOIN tbl_Guests g On b.b_GuestID = g.g_GuestID
                    WHERE 
                        (@CheckInDate < br_CheckOutDate AND @CheckOutDate > br_CheckInDate)
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
                        FirstName = row["g_FirstName"],
                        LastName = row["g_LastName"],
                        Roomnum = row["r_RoomNumber"],
                        CheckInDate = row["br_CheckInDate"],
                        CheckOutDate = row["br_CheckOutDate"],
                        BookingStatus = row["b_BookingStatus"],
                        TotalMoney = row["b_TotalMoney"],
                        Deposit = row["b_Deposit"],
                        Priceperhour = row["r_PricePerHour"]
                    }).ToList<object>();

                    //_cache.Set(cacheKey, bookings, TimeSpan.FromMinutes(1));

                    return Ok(new { bookings });
                }
            }
        }

        // POST Methods
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
                            BrId = Guid.NewGuid(),
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
                    _cache.InvalidateBookingCache();

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
                        BrId = Guid.NewGuid(),
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
                _cache.InvalidateBookingCache();

                return Ok(new { data = "Advance booking successful" });
            }
        }

        [HttpPost]
        [Route("Checkin")]
        public IActionResult Checkin(Guid id)
        {
            dbc.Database.ExecuteSqlRaw("EXEC pro_check_in {0}", id);
            _cache.InvalidateBookingCache();
            return NoContent();
        }

        [HttpPost]
        [Route("Checkout")]
        public IActionResult Checkout([FromBody] CheckoutRequest request)
        {
            decimal formattedTotal = Math.Round(request.Total, 2);
            dbc.Database.ExecuteSql($"EXEC pro_check_out {request.Id}, {request.PayMethod}, {formattedTotal}");
            _cache.InvalidateBookingCache();
            return NoContent();
        }

        [HttpPost]
        [Route("Cancelbooking")]
        public IActionResult Cancelbooking([FromBody] CheckoutRequest request)
        {
            decimal formattedTotal = Math.Round(request.Total, 2);
            dbc.Database.ExecuteSql($"EXEC pro_cancel_booking {request.Id}, {request.PayMethod}, {formattedTotal}");
            _cache.InvalidateBookingCache();
            return NoContent();
        }

        [HttpGet("GetBookingsByUserId")]
        public async Task<IActionResult> GetBookingsByUserId([FromQuery] string userId)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid guestId))
                {
                    return BadRequest("Invalid user ID format. Please provide a valid GUID.");
                }

                // Query bookings for the given guest ID with room info
                var bookings = await dbc.TblBookings
                    .Where(b => b.BGuestId == guestId)
                    .Join(dbc.TblBookingRooms,
                        b => b.BBookingId,
                        br => br.BrBookingId,
                        (b, br) => new { b, br }) // Intermediate anonymous object
                    .Join(dbc.TblRooms,
                        temp => temp.br.BrRoomId,
                        r => r.RRoomId,
                        (temp, r) => new
                        {
                            BookingID = temp.b.BBookingId,
                            GuestID = temp.b.BGuestId,
                            BookingStatus = temp.b.BBookingStatus,
                            TotalMoney = temp.b.BTotalMoney,
                            Deposit = temp.b.BDeposit,
                            CreatedAt = temp.b.BCreatedAt,
                            RoomID = temp.br.BrRoomId,
                            RoomNumber = r.RRoomNumber,
                            CheckInDate = temp.br.BrCheckInDate,
                            CheckOutDate = temp.br.BrCheckOutDate
                        })
                    .ToListAsync();


                // If no bookings found
                if (bookings == null || !bookings.Any())
                {
                    return NotFound($"No bookings found for guest ID: {userId}");
                }

                // Return the bookings
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to use a proper logging framework)
                return StatusCode(500, $"An error occurred while retrieving bookings: {ex.Message}");
            }
        }
    }
}