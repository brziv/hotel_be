using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageDetailController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public PackageDetailController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetPackageDetailList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblPackageDetails.ToList() });
        }

        [HttpGet]
        [Route("SearchTblPackageDetail")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblPackageDetails
                .Where(item =>
                    item.PdQuantity.ToString().Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblPackageDetail")]
        public ActionResult Them(TblPackageDetail packageDetail)
        {
            dbc.TblPackageDetails.Add(packageDetail);
            dbc.SaveChanges();

            return Ok(new { data = packageDetail });
        }

        [HttpPut]
        [Route("UpdateTblPackageDetail")]
        public ActionResult Sua(Guid pdDetailId, Guid pdPackageId, Guid pdProductId, int pdQuantity)
        {
            TblPackageDetail packageDetail = new TblPackageDetail
            {
                PdDetailId = pdDetailId,
                PdPackageId = pdPackageId,
                PdProductId = pdProductId,
                PdQuantity = pdQuantity
            };
            dbc.TblPackageDetails.Update(packageDetail);
            dbc.SaveChanges();
            return Ok(new { data = packageDetail });
        }

        [HttpDelete]
        [Route("XoaTblPackageDetail")]
        public ActionResult Xoa(Guid pdDetailId)
        {
            TblPackageDetail packageDetail = new TblPackageDetail
            {
                PdDetailId = pdDetailId
            };

            dbc.TblPackageDetails.Remove(packageDetail);
            dbc.SaveChanges();
            return Ok(new { data = packageDetail });
        }
    }
}
