using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        DBCnhom4 dbc;
        public PartnerController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetPartnerList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblPartners.ToList() });
        }

        [HttpGet]
        [Route("SearchTblPartner")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblPartners
                .Where(item =>
                    item.PPartnerName.Contains(s) ||
                    (item.PPartnerType != null && item.PPartnerType.Contains(s)) ||
                    item.PPhoneNumber.Contains(s) ||
                    (item.PEmail != null && item.PEmail.Contains(s)) ||
                    (item.PAddress != null && item.PAddress.Contains(s))
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblPartner")]
        public ActionResult Them(TblPartner partner)
        {
            dbc.TblPartners.Add(partner);
            dbc.SaveChanges();

            return Ok(new { data = partner });
        }

        [HttpPut]
        [Route("UpdateTblPartner")]
        public ActionResult Sua([FromBody] TblPartner updatedPartner)
        {
            var existingPartner = dbc.TblPartners.Find(updatedPartner.PPartnerId);
            if (existingPartner == null)
            {
                return NotFound(new { message = "Partner not found" });
            }

            existingPartner.PPartnerName = updatedPartner.PPartnerName;
            existingPartner.PPartnerType = updatedPartner.PPartnerType;
            existingPartner.PPhoneNumber = updatedPartner.PPhoneNumber;
            existingPartner.PEmail = updatedPartner.PEmail;
            existingPartner.PAddress = updatedPartner.PAddress;

            dbc.TblPartners.Update(existingPartner);
            dbc.SaveChanges();

            return Ok(new { data = existingPartner });
        }

        [HttpDelete]
        [Route("XoaTblPartner")]
        public ActionResult Xoa(Guid pPartnerId)
        {
            var partner = dbc.TblPartners.Find(pPartnerId);

            if (partner == null)
            {
                return NotFound(new { message = "Partner not found" });
            }

            dbc.TblPartners.Remove(partner);
            dbc.SaveChanges();

            return Ok(new { data = partner });
        }
    }
}
