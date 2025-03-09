using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportGoodsDetailController : ControllerBase
    {
        DBCnhom4 dbc;
        public ImportGoodsDetailController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetImportGoodsDetailList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblImportGoodsDetails.ToList() });
        }

        [HttpGet]
        [Route("SearchTblImportGoodsDetail")]
        public ActionResult TimKiem(string s)
        {
            string searchTerm = s.ToLower();

            var results = dbc.TblImportGoodsDetails
                .Where(item =>
                    item.IgdId.ToString().Contains(s) ||
                    item.IgdImportId.ToString().Contains(s) ||
                    item.IgdGoodsId.ToString().Contains(s) ||
                    item.IgdQuantity.ToString().Contains(s) ||
                    item.IgdCostPrice.ToString().Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblImportGoodsDetail")]
        public ActionResult Them(Guid igdId, Guid igdImportId, Guid igdGoodsId, int igdQuantity, decimal igdCostPrice)
        {
            TblImportGoodsDetail ImportGoodsDetail = new TblImportGoodsDetail
            {
                IgdId = igdId,
                IgdImportId = igdImportId,
                IgdGoodsId = igdGoodsId,
                IgdQuantity = igdQuantity,
                IgdCostPrice = igdCostPrice
            };

            dbc.TblImportGoodsDetails.Add(ImportGoodsDetail);
            dbc.SaveChanges();

            return Ok(new { data = ImportGoodsDetail });
        }

        [HttpPut]
        [Route("UpdateTblImportGoodsDetail")]
        public ActionResult Sua(Guid igdId, Guid igdImportId, Guid igdGoodsId, int igdQuantity, decimal igdCostPrice)
        {
            TblImportGoodsDetail ImportGoodsDetail = new TblImportGoodsDetail
            {
                IgdId = igdId,
                IgdImportId = igdImportId,
                IgdGoodsId = igdGoodsId,
                IgdQuantity = igdQuantity,
                IgdCostPrice = igdCostPrice
            };
            dbc.TblImportGoodsDetails.Update(ImportGoodsDetail);
            dbc.SaveChanges();
            return Ok(new { data = ImportGoodsDetail });
        }

        [HttpDelete]
        [Route("XoaTblImportGoodsDetail")]
        public ActionResult Xoa(Guid igdId)
        {
            TblImportGoodsDetail ImportGoodsDetail = new TblImportGoodsDetail
            {
                IgdId = igdId,
            };

            dbc.TblImportGoodsDetails.Remove(ImportGoodsDetail);
            dbc.SaveChanges();
            return Ok(new { data = ImportGoodsDetail });
        }
    }
}
