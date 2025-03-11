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
        public ActionResult Sua(Guid pPartnerId, string pPartnerName, string? pPartnerType, string pPhoneNumber, string? pEmail, string? pAddress)
        {
            TblPartner Partner = new TblPartner
            {
                PPartnerId = pPartnerId,
                PPartnerName = pPartnerName,
                PPartnerType = pPartnerType,
                PPhoneNumber = pPhoneNumber,
                PEmail = pEmail,
                PAddress = pAddress
            };
            dbc.TblPartners.Update(Partner);
            dbc.SaveChanges();
            return Ok(new { data = Partner });
        }

        [HttpDelete]
        [Route("XoaTblPartner")]
        public ActionResult Xoa(Guid pPartnerId)
        {
            TblPartner Partner = new TblPartner
            {
                PPartnerId = pPartnerId,
            };

            dbc.TblPartners.Remove(Partner);
            dbc.SaveChanges();
            return Ok(new { data = Partner });
        }
    }
}
