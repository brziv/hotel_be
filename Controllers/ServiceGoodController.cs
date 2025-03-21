﻿using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceGoodController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public ServiceGoodController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetServiceGoodList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblServiceGoods.ToList() });
        }

        [HttpGet]
        [Route("SearchTblServiceGood")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblServiceGoods
                .Where(item =>
                    item.SgQuantity.ToString().Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblServiceGood")]
        public ActionResult Them(TblServiceGood serviceGood)
        {
            dbc.TblServiceGoods.Add(serviceGood);
            dbc.SaveChanges();

            return Ok(new { data = serviceGood });
        }

        [HttpPut]
        [Route("UpdateTblServiceGood")]
        public ActionResult Sua(Guid sgServiceGoodsId, Guid sgServiceId, Guid sgGoodsId, int sgQuantity)
        {
            TblServiceGood ServiceGood = new TblServiceGood
            {
                SgServiceGoodsId = sgServiceGoodsId,
                SgServiceId = sgServiceId,
                SgGoodsId = sgGoodsId,
                SgQuantity = sgQuantity
            };
            dbc.TblServiceGoods.Update(ServiceGood);
            dbc.SaveChanges();
            return Ok(new { data = ServiceGood });
        }

        [HttpDelete]
        [Route("XoaTblServiceGood")]
        public ActionResult Xoa(Guid sgServiceGoodsId)
        {
            TblServiceGood ServiceGood = new TblServiceGood
            {
                SgServiceGoodsId = sgServiceGoodsId,
            };

            dbc.TblServiceGoods.Remove(ServiceGood);
            dbc.SaveChanges();
            return Ok(new { data = ServiceGood });
        }
    }
}
