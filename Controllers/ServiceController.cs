using hotel_be.DTOs;
using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public ServiceController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetServiceList")]
        public ActionResult GetServiceList()
        {
            var services = dbc.TblServices
                .Include(s => s.TblServiceGoods)
                    .ThenInclude(sg => sg.SgGoods)
                .Select(s => new ServiceGetDto
                {
                    SServiceId = s.SServiceId,
                    SServiceName = s.SServiceName,
                    SServiceCostPrice = s.TblServiceGoods.Sum(sg => sg.SgGoods.GCostPrice * sg.SgQuantity),
                    SServiceSellPrice = s.TblServiceGoods.Sum(sg => sg.SgGoods.GSellingPrice * sg.SgQuantity),
                    GoodsInfo = string.Join("\n", s.TblServiceGoods.Select(sg => $"{sg.SgQuantity} {sg.SgGoods.GGoodsName}")),
                    ServiceGoods = s.TblServiceGoods.Select(sg => new ServiceGoodDto
                    {
                        SgGoodsId = sg.SgGoodsId,
                        SgQuantity = sg.SgQuantity
                    }).ToList()
                })
                .ToList();

            return Ok(new { data = services });
        }

        [HttpGet]
        [Route("SearchTblService")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblServices
                .Where(item =>
                    item.SServiceName.Contains(s) ||
                    item.SServiceCostPrice.ToString().Contains(s) ||
                    item.SServiceSellPrice.ToString().Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblService")]
        public async Task<ActionResult> InsertTblService([FromBody] ServiceRequestDto serviceDto)
        {
            if (serviceDto == null || string.IsNullOrEmpty(serviceDto.SServiceName) || !serviceDto.ServiceGoods.Any())
            {
                return BadRequest("Invalid service data");
            }

            var service = new TblService
            {
                SServiceId = Guid.NewGuid(),
                SServiceName = serviceDto.SServiceName,
                SServiceCostPrice = 0,
                SServiceSellPrice = 0,
                TblServiceGoods = new List<TblServiceGood>()
            };

            foreach (var serviceGood in serviceDto.ServiceGoods)
            {
                var good = await dbc.TblGoods.FindAsync(serviceGood.SgGoodsId);
                if (good == null)
                {
                    return BadRequest($"Good with ID {serviceGood.SgGoodsId} not found");
                }

                service.TblServiceGoods.Add(new TblServiceGood
                {
                    SgServiceGoodsId = Guid.NewGuid(),
                    SgServiceId = service.SServiceId,
                    SgGoodsId = serviceGood.SgGoodsId,
                    SgQuantity = serviceGood.SgQuantity
                });

                service.SServiceCostPrice += good.GCostPrice * serviceGood.SgQuantity;
                service.SServiceSellPrice += good.GSellingPrice * serviceGood.SgQuantity;
            }

            dbc.TblServices.Add(service);
            await dbc.SaveChangesAsync();

            return Ok(new { data = service.SServiceId });
        }

        [HttpPut]
        [Route("UpdateTblService")]
        public async Task<ActionResult> UpdateTblService([FromBody] ServiceRequestDto serviceDto)
        {
            if (serviceDto == null || !serviceDto.ServiceGoods.Any())
            {
                return BadRequest("Invalid service data");
            }

            var service = await dbc.TblServices
                .Include(s => s.TblServiceGoods)
                .FirstOrDefaultAsync(s => s.SServiceId == serviceDto.SServiceId);

            if (service == null)
            {
                return NotFound("Service not found");
            }

            service.SServiceName = serviceDto.SServiceName;

            // Remove existing service goods
            dbc.TblServiceGoods.RemoveRange(service.TblServiceGoods);
            service.TblServiceGoods.Clear();

            // Recalculate prices and add new service goods
            service.SServiceCostPrice = 0;
            service.SServiceSellPrice = 0;

            foreach (var serviceGood in serviceDto.ServiceGoods)
            {
                var good = await dbc.TblGoods.FindAsync(serviceGood.SgGoodsId);
                if (good == null)
                {
                    return BadRequest($"Good with ID {serviceGood.SgGoodsId} not found");
                }

                service.TblServiceGoods.Add(new TblServiceGood
                {
                    SgServiceGoodsId = Guid.NewGuid(),
                    SgServiceId = service.SServiceId,
                    SgGoodsId = serviceGood.SgGoodsId,
                    SgQuantity = serviceGood.SgQuantity
                });

                service.SServiceCostPrice += good.GCostPrice * serviceGood.SgQuantity;
                service.SServiceSellPrice += good.GSellingPrice * serviceGood.SgQuantity;
            }

            await dbc.SaveChangesAsync();
            return Ok(new { data = service.SServiceId });
        }

        [HttpDelete]
        [Route("XoaTblService")]
        public async Task<ActionResult> XoaTblService([FromQuery] Guid sServiceId)
        {
            var service = await dbc.TblServices
                .Include(s => s.TblServiceGoods)
                .FirstOrDefaultAsync(s => s.SServiceId == sServiceId);

            if (service == null)
            {
                return NotFound("Service not found");
            }

            // Remove TblServiceGood entries (foreign key)
            dbc.TblServiceGoods.RemoveRange(service.TblServiceGoods);
            dbc.TblServices.Remove(service);

            await dbc.SaveChangesAsync();
            return Ok(new { message = "Service deleted successfully" });
        }
    }
}
