using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        DBCnhom4 dbc;
        public ServiceController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetServiceList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblServices.ToList() });
        }

        [HttpGet]
        [Route("SearchTblService")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblServices
                .Where(item =>
                    item.SServiceId.ToString().Contains(s) ||
                    item.SServiceName.Contains(s) ||
                    item.SServiceCostPrice.ToString().Contains(s) ||
                    item.SServiceSellPrice.ToString().Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblService")]
        public ActionResult Them(TblService service)
        {
            dbc.TblServices.Add(service);
            dbc.SaveChanges();

            return Ok(new { data = service });
        }

        [HttpPut]
        [Route("UpdateTblService")]
        public ActionResult Sua(Guid sServiceId, string sServiceName, decimal sServiceCostPrice, decimal sServiceSellPrice)
        {
            TblService Service = new TblService
            {
                SServiceId = sServiceId,
                SServiceName = sServiceName,
                SServiceCostPrice = sServiceCostPrice,
                SServiceSellPrice = sServiceSellPrice
            };
            dbc.TblServices.Update(Service);
            dbc.SaveChanges();
            return Ok(new { data = Service });
        }

        [HttpDelete]
        [Route("XoaTblService")]
        public ActionResult Xoa(Guid sServiceId)
        {
            TblService Service = new TblService
            {
                SServiceId = sServiceId,
            };

            dbc.TblServices.Remove(Service);
            dbc.SaveChanges();
            return Ok(new { data = Service });
        }
    }
}
