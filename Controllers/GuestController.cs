using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public GuestController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetGuestList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblGuests.ToList() });
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

        [HttpDelete]
        [Route("XoaTblGuest")]
        public ActionResult Xoa(Guid gGuestId)
        {
            var guest = dbc.TblGuests.Find(gGuestId);

            if (guest == null)
            {
                return NotFound(new { message = "Guest not found" });
            }

            try
            {
                dbc.TblGuests.Remove(guest);
                dbc.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { data = guest });
        }
    }
}
