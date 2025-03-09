using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("GetImportGoodsDetailList/{importId}")]
        public async Task<IActionResult> GetImportGoodsDetailList(Guid importId)
        {
            var details = await dbc.TblImportGoodsDetails
                .Where(d => d.IgdImportId == importId)
                .Join(dbc.TblGoods,
                      detail => detail.IgdGoodsId,
                      goods => goods.GGoodsId,
                      (detail, goods) => new
                      {
                          GoodsName = goods.GGoodsName,
                          Quantity = detail.IgdQuantity,
                          CostPrice = detail.IgdCostPrice
                      })
                .ToListAsync();

            return Ok(new { data = details });
        }

        [HttpGet]
        [Route("SearchTblImportGoodsDetail")]
        public ActionResult TimKiem(string s)
        {
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
        public ActionResult Them(TblImportGoodsDetail importGoodsDetail)
        {
            dbc.TblImportGoodsDetails.Add(importGoodsDetail);
            dbc.SaveChanges();

            return Ok(new { data = importGoodsDetail });
        }

        [HttpPut]
        [Route("UpdateTblImportGoodsDetail")]
        public ActionResult Sua([FromBody] TblImportGoodsDetail updatedImportGoodsDetail)
        {
            var existingImportGoodsDetail = dbc.TblImportGoodsDetails.Find(updatedImportGoodsDetail.IgdId);
            if (existingImportGoodsDetail == null)
            {
                return NotFound(new { message = "Import goods detail not found" });
            }

            existingImportGoodsDetail.IgdImportId = updatedImportGoodsDetail.IgdImportId;
            existingImportGoodsDetail.IgdGoodsId = updatedImportGoodsDetail.IgdGoodsId;
            existingImportGoodsDetail.IgdQuantity = updatedImportGoodsDetail.IgdQuantity;
            existingImportGoodsDetail.IgdCostPrice = updatedImportGoodsDetail.IgdCostPrice;

            dbc.TblImportGoodsDetails.Update(existingImportGoodsDetail);
            dbc.SaveChanges();

            return Ok(new { data = existingImportGoodsDetail });
        }

        [HttpDelete]
        [Route("XoaTblImportGoodsDetail")]
        public ActionResult Xoa(Guid igdId)
        {
            var importGoodsDetail = dbc.TblImportGoodsDetails.Find(igdId);
            if (importGoodsDetail == null)
            {
                return NotFound(new { message = "Import goods detail not found" });
            }

            dbc.TblImportGoodsDetails.Remove(importGoodsDetail);
            dbc.SaveChanges();

            return Ok(new { data = importGoodsDetail });
        }
    }
}
