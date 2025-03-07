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

        [HttpPost]
        [Route("SearchTblImportGood")]
        public ActionResult TimKiem(string s)
        {
            string searchTerm = s.ToLower();

            var results = dbc.TblImportGoods
                .Where(item =>
                    item.IgImportId.ToString().Contains(s) ||
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
        public ActionResult Them(Guid igImportId, decimal igSumPrice, string igCurrency, DateTime? igImportDate, string? igSupplier)
        {
            TblImportGood ImportGood = new TblImportGood
            {
                IgImportId = igImportId,
                IgSumPrice = igSumPrice,
                IgCurrency = igCurrency,
                IgImportDate = igImportDate,
                IgSupplier = igSupplier
            };

            dbc.TblImportGoods.Add(ImportGood);
            dbc.SaveChanges();

            return Ok(new { data = ImportGood });
        }

        [HttpPost]
        [Route("UpdateTblImportGood")]
        public ActionResult Xoa(Guid igImportId, decimal igSumPrice, string igCurrency, DateTime? igImportDate, string? igSupplier)
        {
            TblImportGood ImportGood = new TblImportGood
            {
                IgImportId = igImportId,
                IgSumPrice = igSumPrice,
                IgCurrency = igCurrency,
                IgImportDate = igImportDate,
                IgSupplier = igSupplier
            };
            dbc.TblImportGoods.Update(ImportGood);
            dbc.SaveChanges();
            return Ok(new { data = ImportGood });
        }

        [HttpPost]
        [Route("XoaTblImportGood")]
        public ActionResult Xoa(Guid igImportId)
        {
            TblImportGood ImportGood = new TblImportGood
            {
                IgImportId = igImportId
            };

            dbc.TblImportGoods.Remove(ImportGood);
            dbc.SaveChanges();
            return Ok(new { data = ImportGood });
        }
    }
}
