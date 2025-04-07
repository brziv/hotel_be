using hotel_be.DTOs;
using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IgdController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public IgdController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
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
                    PProductName = igd.IgdGoods != null ? igd.IgdGoods.PProductName : null,
                    IgSupplier = igd.IgdImport != null ? igd.IgdImport.IgSupplier : null,
                    IgImportDate = igd.IgdImport != null ? igd.IgdImport.IgImportDate : null
                })
                .ToList();

            return Ok(new { data = importDetails });
        }

        [HttpGet("GetImportGoodsDetailListByImport/{importId}")]
        public async Task<IActionResult> GetImport(Guid importId)
        {
            try
            {
                var details = await dbc.TblImportGoodsDetails
                    .Where(d => d.IgdImportId == importId)
                    .Join(dbc.TblProducts,
                          detail => detail.IgdGoodsId,
                          goods => goods.PProductId,
                          (detail, goods) => new ImportGoodsByImportDto
                          {
                              IgdId = detail.IgdId,
                              IgdQuantity = detail.IgdQuantity,
                              IgdCostPrice = detail.IgdCostPrice,
                              IgdGoodsId = detail.IgdGoodsId,
                              PProductName = goods.PProductName
                          })
                    .ToListAsync();

                if (details == null || details.Count == 0)
                {
                    return NotFound(new { message = $"No details found for import ID {importId}" });
                }

                return Ok(new { data = details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching import details", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetImportGoodsDetailListByGood/{goodID}")]
        public ActionResult GetGood(Guid goodID)
        {
            var details = dbc.TblImportGoodsDetails
                .Where(detail => detail.IgdGoodsId == goodID)
                .Select(detail => new ImportGoodsByGoodDto
                {
                    IgdId = detail.IgdId,
                    IgdQuantity = detail.IgdQuantity,
                    IgdCostPrice = detail.IgdCostPrice,
                    IgdImportId = detail.IgdImportId,
                    IgdGoodsId = detail.IgdGoodsId,
                    IgImportDate = detail.IgdImport != null ? detail.IgdImport.IgImportDate : null,
                    IgSupplier = detail.IgdImport != null ? detail.IgdImport.IgSupplier : null
                })
                .ToList();

            if (details == null || details.Count == 0)
            {
                return NotFound(new { message = "No import goods details found for the given GoodID." });
            }

            return Ok(new { data = details });
        }

        [HttpPost]
        [Route("InsertTblImportGoodsDetail")]
        public ActionResult Them(TblImportGoodsDetail importGoodsDetail)
        {
            dbc.TblImportGoodsDetails.Add(importGoodsDetail);
            dbc.SaveChanges();

            return Ok(new { data = importGoodsDetail });
        }
    }
}
