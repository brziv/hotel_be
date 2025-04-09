using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using hotel_be.DTOs;
using Microsoft.AspNetCore.Identity;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        private readonly UserManager<IdentityUser> _userManager;
        public GuestController(DBCnhom4 dbc_in, UserManager<IdentityUser> userManager)
        {
            dbc = dbc_in;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetGuestList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblGuests.ToList() });
        }

        [HttpGet("GetGuestByUserId")]
        public async Task<IActionResult> GetGuestByUsername(Guid userId)
        {
            try
            {
                var guest = await dbc.TblGuests
                    .Where(g => g.GGuestId == userId)
                    .Select(g => new
                    {
                        g.GGuestId,
                        g.GFirstName,
                        g.GLastName,
                        g.GEmail,
                        g.GPhoneNumber
                    })
                    .FirstOrDefaultAsync();

                if (guest == null)
                {
                    return NotFound(new { code = 404, msg = "Guest not found" });
                }

                return Ok(new { code = 100, data = guest });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("SearchTblGuest")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblGuests
                .Where(item =>
                    item.GFirstName.Contains(s) ||
                    item.GLastName.Contains(s) ||
                    (item.GEmail != null && item.GEmail.Contains(s)) ||
                    item.GPhoneNumber.Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblGuest")]
        public ActionResult Them(TblGuest guest)
        {
            dbc.TblGuests.Add(guest);
            dbc.SaveChanges();

            return Ok(new { data = guest });
        }

        [HttpPut]
        [Route("UpdateTblGuest")]
        public ActionResult Sua([FromBody] TblGuest updatedGuest)
        {
            var existingGuest = dbc.TblGuests.Find(updatedGuest.GGuestId);
            if (existingGuest == null)
            {
                return NotFound(new { message = "Guest not found" });
            }

            existingGuest.GFirstName = updatedGuest.GFirstName;
            existingGuest.GLastName = updatedGuest.GLastName;
            existingGuest.GEmail = updatedGuest.GEmail;
            existingGuest.GPhoneNumber = updatedGuest.GPhoneNumber;

            dbc.TblGuests.Update(existingGuest);
            dbc.SaveChanges();

            return Ok(new { data = existingGuest });
        }

        [HttpPut("UpdateGuestProfile")]
        public async Task<IActionResult> UpdateTblGuest([FromBody] UpdateGuestDto model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var guest = await dbc.TblGuests
                    .FirstOrDefaultAsync(g => g.GGuestId == model.GGuestId);

                if (guest == null)
                {
                    return NotFound(new { code = 404, msg = "Guest not found" });
                }

                guest.GFirstName = model.GFirstName;
                guest.GLastName = model.GLastName;
                guest.GEmail = model.GEmail;
                guest.GPhoneNumber = model.GPhoneNumber;

                await dbc.SaveChangesAsync();

                return Ok(new { code = 100, msg = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, msg = ex.Message });
            }
        }

        [HttpDelete]
        [Route("XoaTblGuest")]
        public async Task<ActionResult> Xoa(Guid gGuestId)
        {
            try
            {
                // Find the guest including the linked User
                var guest = await dbc.TblGuests
                    .Include(g => g.User)
                    .FirstOrDefaultAsync(g => g.GGuestId == gGuestId);

                if (guest == null)
                {
                    return NotFound(new { message = "Guest not found" });
                }

                // Check if there's a linked user account
                if (guest.User != null)
                {
                    // Delete the IdentityUser account
                    var userDeleteResult = await _userManager.DeleteAsync(guest.User);
                    if (!userDeleteResult.Succeeded)
                    {
                        var errors = string.Join("; ", userDeleteResult.Errors.Select(e => e.Description));
                        return BadRequest(new { message = $"Failed to delete user account: {errors}" });
                    }
                }

                // Delete the guest record
                dbc.TblGuests.Remove(guest);
                await dbc.SaveChangesAsync();

                return Ok(new { data = guest });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
