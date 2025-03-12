using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportGoodController : ControllerBase
    {
        DBCnhom4 dbc;
        public ImportGoodController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetImportGoodList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblImportGoods.ToList() });
        }

        [HttpGet]
        [Route("SearchTblImportGood")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblImportGoods
                .Where(item =>
                    item.IgSumPrice.ToString().Contains(s) ||
                    item.IgCurrency.Contains(s) ||
                    (item.IgImportDate.HasValue && item.IgImportDate.Value.ToString().Contains(s)) ||
                    (item.IgSupplier != null && item.IgSupplier.Contains(s))
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblImportGood")]
        public async Task<ActionResult> Them(TblImportGood importGood)
        {
            try
            {
                dbc.TblImportGoods.Add(importGood);
                await dbc.SaveChangesAsync();
                return Ok(new { data = importGood });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateTblImportGood")]
        public async Task<ActionResult> Sua([FromBody] TblImportGood updatedImportGood)
        {
            try
            {
                var existingImportGood = await dbc.TblImportGoods.FindAsync(updatedImportGood.IgImportId);
                if (existingImportGood == null)
                {
                    return NotFound(new { message = "Import good not found" });
                }

                existingImportGood.IgSumPrice = updatedImportGood.IgSumPrice;
                existingImportGood.IgCurrency = updatedImportGood.IgCurrency;
                existingImportGood.IgImportDate = updatedImportGood.IgImportDate;
                existingImportGood.IgSupplier = updatedImportGood.IgSupplier;

                dbc.TblImportGoods.Update(existingImportGood);
                await dbc.SaveChangesAsync();

                return Ok(new { data = existingImportGood });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("XoaTblImportGood")]
        public async Task<ActionResult> Xoa(Guid igImportId)
        {
            try
            {
                var importGood = await dbc.TblImportGoods.FindAsync(igImportId);
                if (importGood == null)
                {
                    return NotFound(new { message = "Import good not found" });
                }

                var details = dbc.TblImportGoodsDetails.Where(d => d.IgdImportId == igImportId);
                dbc.TblImportGoodsDetails.RemoveRange(details);

                dbc.TblImportGoods.Remove(importGood);
                await dbc.SaveChangesAsync();

                return Ok(new { data = importGood });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
