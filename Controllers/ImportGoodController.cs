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
        public ActionResult Them(TblImportGood importGood)
        {
            dbc.TblImportGoods.Add(importGood);
            dbc.SaveChanges();

            return Ok(new { data = importGood });
        }

        [HttpPut]
        [Route("UpdateTblImportGood")]
        public ActionResult Sua([FromBody] TblImportGood updatedImportGood)
        {
            var existingImportGood = dbc.TblImportGoods.Find(updatedImportGood.IgImportId);
            if (existingImportGood == null)
            {
                return NotFound(new { message = "Import good not found" });
            }

            existingImportGood.IgSumPrice = updatedImportGood.IgSumPrice;
            existingImportGood.IgCurrency = updatedImportGood.IgCurrency;
            existingImportGood.IgImportDate = updatedImportGood.IgImportDate;
            existingImportGood.IgSupplier = updatedImportGood.IgSupplier;

            dbc.TblImportGoods.Update(existingImportGood);
            dbc.SaveChanges();

            return Ok(new { data = existingImportGood });
        }

        [HttpDelete]
        [Route("XoaTblImportGood")]
        public ActionResult Xoa(Guid igImportId)
        {
            var importGood = dbc.TblImportGoods.Find(igImportId);
            if (importGood == null)
            {
                return NotFound(new { message = "Import good not found" });
            }

            dbc.TblImportGoods.Remove(importGood);
            dbc.SaveChanges();

            return Ok(new { data = importGood });
        }
    }
}
