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

        [HttpPost]
        [Route("SearchTblGuest")]
        public ActionResult TimKiem(string s)
        {
            string searchTerm = s.ToLower();

            var results = dbc.TblGuests
                .Where(item =>
                    item.GGuestId.ToString().Contains(s) ||
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
        public ActionResult Them(Guid gGuestId, string gFirstName, string gLastName, string? gEmail, string gPhoneNumber)
        {
            TblGuest Guest = new TblGuest
            {
                GGuestId = gGuestId,
                GFirstName = gFirstName,
                GLastName = gLastName,
                GEmail = gEmail,
                GPhoneNumber = gPhoneNumber
            };

            dbc.TblGuests.Add(Guest);
            dbc.SaveChanges();

            return Ok(new { data = Guest });
        }

        [HttpPost]
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

        [HttpPost]
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
