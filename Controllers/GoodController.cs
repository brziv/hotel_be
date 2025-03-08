using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodController : ControllerBase
    {
        DBCnhom4 dbc;
        public GoodController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetGoodList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblGoods.ToList() });
        }

        [HttpGet]
        [Route("SearchTblGood")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblGoods
                .Where(item =>
                    item.GGoodsId.ToString().Contains(s) ||
                    item.GGoodsName.Contains(s) ||
                    (item.GCategory != null && item.GCategory.Contains(s)) ||
                    (item.GQuantity.HasValue && item.GQuantity.Value.ToString().Contains(s)) ||
                    (item.GUnit != null && item.GUnit.Contains(s)) ||
                    item.GCostPrice.ToString().Contains(s) ||
                    item.GSellingPrice.ToString().Contains(s) ||
                    item.GCurrency.Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblGood")]
        public ActionResult Them(TblGood good)
        {
            dbc.TblGoods.Add(good);
            dbc.SaveChanges();

            return Ok(new { data = good });
        }

        [HttpPost]
        [Route("UpdateTblGood")]
        public ActionResult Sua(Guid gGoodsId, string gGoodsName, string? gCategory, int? gQuantity, string? gUnit, decimal gCostPrice, decimal gSellingPrice, string gCurrency)
        {
            TblGood Good = new TblGood
            {
                GGoodsId = gGoodsId,
                GGoodsName = gGoodsName,
                GCategory = gCategory,
                GQuantity = gQuantity,
                GUnit = gUnit,
                GCostPrice = gCostPrice,
                GSellingPrice = gSellingPrice,
                GCurrency = gCurrency
            };
            dbc.TblGoods.Update(Good);
            dbc.SaveChanges();
            return Ok(new { data = Good });
        }

        [HttpPost]
        [Route("XoaTblGood")]
        public ActionResult Xoa(Guid gGoodsId)
        {
            TblGood Good = new TblGood
            {
                GGoodsId = gGoodsId
            };

            dbc.TblGoods.Remove(Good);
            dbc.SaveChanges();
            return Ok(new { data = Good });
        }
    }
}
