﻿using hotel_be.ModelFromDB;
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

        [HttpPut]
        [Route("UpdateTblGood")]
        public ActionResult Sua([FromBody] TblGood updatedGood)
        {
            var existingGood = dbc.TblGoods.Find(updatedGood.GGoodsId);
            if (existingGood == null)
            {
                return NotFound(new { message = "Good not found" });
            }

            existingGood.GGoodsName = updatedGood.GGoodsName;
            existingGood.GCategory = updatedGood.GCategory;
            existingGood.GQuantity = updatedGood.GQuantity;
            existingGood.GUnit = updatedGood.GUnit;
            existingGood.GCostPrice = updatedGood.GCostPrice;
            existingGood.GSellingPrice = updatedGood.GSellingPrice;
            existingGood.GCurrency = updatedGood.GCurrency;

            dbc.TblGoods.Update(existingGood);
            dbc.SaveChanges();

            return Ok(new { data = existingGood });
        }

        [HttpDelete]
        [Route("XoaTblGood")]
        public ActionResult Xoa(Guid gGoodsId)
        {
            var good = dbc.TblGoods.Find(gGoodsId);
            if (good == null)
            {
                return NotFound(new { message = "Good not found" });
            }

            dbc.TblGoods.Remove(good);
            dbc.SaveChanges();

            return Ok(new { data = good });
        }
    }
}
