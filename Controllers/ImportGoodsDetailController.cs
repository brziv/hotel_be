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
        public class ImportGoodsDetailDto
        {
            public Guid IgdId { get; set; }
            public int IgdQuantity { get; set; }
            public decimal IgdCostPrice { get; set; }
            public string? GoodsName { get; set; }
            public string? Supplier { get; set; }
            public DateTime? ImportDate { get; set; }
        }

        [HttpGet]
        [Route("GetImportGoodsDetailList")]
        public ActionResult Get()
        {
            var importDetails = dbc.TblImportGoodsDetails
                .Include(igd => igd.IgdGoods)
                .Include(igd => igd.IgdImport)
                .Select(igd => new ImportGoodsDetailDto
                {
                    IgdId = igd.IgdId,
                    IgdQuantity = igd.IgdQuantity,
                    IgdCostPrice = igd.IgdCostPrice,
                    GoodsName = igd.IgdGoods.GGoodsName,
                    Supplier = igd.IgdImport.IgSupplier,
                    ImportDate = igd.IgdImport.IgImportDate
                })
                .ToList();

            return Ok(new { data = importDetails });
        }

        [HttpGet("GetImportGoodsDetailListByImport/{importId}")]
        public async Task<IActionResult> GetImport(Guid importId)
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
        [Route("GetImportGoodsDetailListByGood/{goodID}")]
        public ActionResult GetGood(Guid goodID)
        {
            var details = dbc.TblImportGoodsDetails
                .Where(detail => detail.IgdGoodsId == goodID)
                .Select(detail => new {
                    detail.IgdId,
                    detail.IgdImportId,
                    detail.IgdGoodsId,
                    detail.IgdQuantity,
                    detail.IgdCostPrice,
                    ImportDate = detail.IgdImport != null ? detail.IgdImport.IgImportDate : null
                })
                .ToList();

            if (details == null || details.Count == 0)
            {
                return NotFound(new { message = "No import goods details found for the given GoodID." });
            }

            return Ok(new { data = details });
        }

        [HttpGet]
        [Route("SearchTblImportGoodsDetail")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblImportGoodsDetails
                .Where(item =>
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
