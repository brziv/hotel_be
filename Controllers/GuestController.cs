using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        DBCnhom4 dbc;
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
        public ActionResult Sua(Guid gGuestId, string gFirstName, string gLastName, string? gEmail, string gPhoneNumber)
        {
            TblGuest Guest = new TblGuest
            {
                GGuestId = gGuestId,
                GFirstName = gFirstName,
                GLastName = gLastName,
                GEmail = gEmail,
                GPhoneNumber = gPhoneNumber
            };
            dbc.TblGuests.Update(Guest);
            dbc.SaveChanges();
            return Ok(new { data = Guest });
        }

        [HttpDelete]
        [Route("XoaTblGuest")]
        public ActionResult Xoa(Guid gGuestId)
        {
            TblGuest Guest = new TblGuest
            {
                GGuestId = gGuestId,
            };

            dbc.TblGuests.Remove(Guest);
            dbc.SaveChanges();
            return Ok(new { data = Guest });
        }
    }
}
