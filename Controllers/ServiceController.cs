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
            var services = dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                    .ThenInclude(sg => sg.PdProduct)
                .Select(s => new ServiceGetDto
                {
                    SServiceId = s.SpPackageId,
                    SServiceName = s.SpPackageName,
                    SServiceCostPrice = s.TblPackageDetails.Sum(sg => sg.PdProduct.PCostPrice * sg.PdQuantity),
                    SServiceSellPrice = s.TblPackageDetails.Sum(sg => sg.PdProduct.PSellingPrice * sg.PdQuantity),
                    GoodsInfo = string.Join("\n", s.TblPackageDetails.Select(sg => $"{sg.PdQuantity} {sg.PdProduct.PProductName}")),
                    ServiceGoods = s.TblPackageDetails.Select(sg => new ServiceGoodDto
                    {
                        SgGoodsId = sg.PdProductId,
                        SgQuantity = sg.PdQuantity
                    }).ToList()
                })
                .ToList();

            return Ok(new { data = services });
        }

        [HttpGet]
        [Route("SearchTblService")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblServicePackages
                .Where(item =>
                    item.SpPackageName.Contains(s) ||
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

            var service = new TblServicePackage
            {
                SpPackageId = Guid.NewGuid(),
                SpPackageName = serviceDto.SServiceName,
                SServiceCostPrice = 0,
                SServiceSellPrice = 0,
                TblPackageDetails = new List<TblPackageDetail>()
            };

            foreach (var serviceGood in serviceDto.ServiceGoods)
            {
                var good = await dbc.TblProducts.FindAsync(serviceGood.SgGoodsId);
                if (good == null)
                {
                    return BadRequest($"Good with ID {serviceGood.SgGoodsId} not found");
                }

                service.TblPackageDetails.Add(new TblPackageDetail
                {
                    PdDetailId = Guid.NewGuid(),
                    PdPackageId = service.SpPackageId,
                    PdProductId = serviceGood.SgGoodsId,
                    PdQuantity = serviceGood.SgQuantity
                });

                service.SServiceCostPrice += good.PCostPrice * serviceGood.SgQuantity;
                service.SServiceSellPrice += good.PSellingPrice * serviceGood.SgQuantity;
            }

            dbc.TblServicePackages.Add(service);
            await dbc.SaveChangesAsync();

            return Ok(new { data = service.SpPackageId });
        }

        [HttpPut]
        [Route("UpdateTblService")]
        public async Task<ActionResult> UpdateTblService([FromBody] ServiceRequestDto serviceDto)
        {
            if (serviceDto == null || !serviceDto.ServiceGoods.Any())
            {
                return BadRequest("Invalid service data");
            }

            var service = await dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                .FirstOrDefaultAsync(s => s.SpPackageId == serviceDto.SServiceId);

            if (service == null)
            {
                return NotFound("Service not found");
            }

            service.SpPackageName = serviceDto.SServiceName;

            // Remove existing service goods
            dbc.TblPackageDetails.RemoveRange(service.TblPackageDetails);
            service.TblPackageDetails.Clear();

            // Recalculate prices and add new service goods
            service.SServiceCostPrice = 0;
            service.SServiceSellPrice = 0;

            foreach (var serviceGood in serviceDto.ServiceGoods)
            {
                var good = await dbc.TblProducts.FindAsync(serviceGood.SgGoodsId);
                if (good == null)
                {
                    return BadRequest($"Good with ID {serviceGood.SgGoodsId} not found");
                }

                service.TblPackageDetails.Add(new TblPackageDetail
                {
                    PdDetailId = Guid.NewGuid(),
                    PdPackageId = service.SpPackageId,
                    PdProductId = serviceGood.SgGoodsId,
                    PdQuantity = serviceGood.SgQuantity
                });

                service.SServiceCostPrice += good.PCostPrice * serviceGood.SgQuantity;
                service.SServiceSellPrice += good.PSellingPrice * serviceGood.SgQuantity;
            }

            await dbc.SaveChangesAsync();
            return Ok(new { data = service.SpPackageId });
        }

        [HttpDelete]
        [Route("XoaTblService")]
        public async Task<ActionResult> XoaTblService([FromQuery] Guid spPackageId)
        {
            var service = await dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                .FirstOrDefaultAsync(s => s.SpPackageId == spPackageId);

            if (service == null)
            {
                return NotFound("Service not found");
            }

            // Remove TblPackageDeatil entries (foreign key)
            dbc.TblPackageDetails.RemoveRange(service.TblPackageDetails);
            dbc.TblServicePackages.Remove(service);

            await dbc.SaveChangesAsync();
            return Ok(new { message = "Service deleted successfully" });
        }
    }
}
