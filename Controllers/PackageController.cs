using hotel_be.DTOs;
using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public PackageController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetPackageList")]
        public ActionResult GetPackageList()
        {
            var packages = dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                    .ThenInclude(sg => sg.PdProduct)
                .Select(s => new PackageGetDto
                {
                    SpPackageId = s.SpPackageId,
                    SpPackageName = s.SpPackageName,
                    SServiceCostPrice = s.TblPackageDetails.Sum(sg => sg.PdProduct.PCostPrice * sg.PdQuantity),
                    SServiceSellPrice = s.TblPackageDetails.Sum(sg => sg.PdProduct.PSellingPrice * sg.PdQuantity),
                    ProductsInfo = string.Join("\n", s.TblPackageDetails.Select(sg => $"{sg.PdQuantity} {sg.PdProduct.PProductName}")),
                    PackageDetails = s.TblPackageDetails.Select(sg => new PackageDetailDto
                    {
                        PdProductId = sg.PdProductId,
                        PdQuantity = sg.PdQuantity
                    }).ToList()
                })
                .ToList();

            return Ok(new { data = packages });
        }

        [HttpGet]
        [Route("SearchTblPackage")]
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
        [Route("InsertTblPackage")]
        public async Task<ActionResult> InsertTblService([FromBody] PackageRequestDto packageDto)
        {
            if (packageDto == null || string.IsNullOrEmpty(packageDto.SpPackageName) || !packageDto.PackageDetails.Any())
            {
                return BadRequest("Invalid package data");
            }

            var service = new TblServicePackage
            {
                SpPackageId = Guid.NewGuid(),
                SpPackageName = packageDto.SpPackageName,
                SServiceCostPrice = 0,
                SServiceSellPrice = 0,
                TblPackageDetails = new List<TblPackageDetail>()
            };

            foreach (var packageDetail in packageDto.PackageDetails)
            {
                var product = await dbc.TblProducts.FindAsync(packageDetail.PdProductId);
                if (product == null)
                {
                    return BadRequest($"product with ID {packageDetail.PdProductId} not found");
                }

                service.TblPackageDetails.Add(new TblPackageDetail
                {
                    PdDetailId = Guid.NewGuid(),
                    PdPackageId = service.SpPackageId,
                    PdProductId = packageDetail.PdProductId,
                    PdQuantity = packageDetail.PdQuantity
                });

                service.SServiceCostPrice += product.PCostPrice * packageDetail.PdQuantity;
                service.SServiceSellPrice += product.PSellingPrice * packageDetail.PdQuantity;
            }

            dbc.TblServicePackages.Add(service);
            await dbc.SaveChangesAsync();

            return Ok(new { data = service.SpPackageId });
        }

        [HttpPut]
        [Route("UpdateTblPackage")]
        public async Task<ActionResult> UpdateTblService([FromBody] PackageRequestDto packageDto)
        {
            if (packageDto == null || !packageDto.PackageDetails.Any())
            {
                return BadRequest("Invalid package data");
            }

            var service = await dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                .FirstOrDefaultAsync(s => s.SpPackageId == packageDto.SpPackageId);

            if (service == null)
            {
                return NotFound("Package not found");
            }

            service.SpPackageName = packageDto.SpPackageName;

            // Remove existing package details
            dbc.TblPackageDetails.RemoveRange(service.TblPackageDetails);
            service.TblPackageDetails.Clear();

            // Recalculate prices and add new package details
            service.SServiceCostPrice = 0;
            service.SServiceSellPrice = 0;

            foreach (var packageDetail in packageDto.PackageDetails)
            {
                var product = await dbc.TblProducts.FindAsync(packageDetail.PdProductId);
                if (product == null)
                {
                    return BadRequest($"product with ID {packageDetail.PdProductId} not found");
                }

                service.TblPackageDetails.Add(new TblPackageDetail
                {
                    PdDetailId = Guid.NewGuid(),
                    PdPackageId = service.SpPackageId,
                    PdProductId = packageDetail.PdProductId,
                    PdQuantity = packageDetail.PdQuantity
                });

                service.SServiceCostPrice += product.PCostPrice * packageDetail.PdQuantity;
                service.SServiceSellPrice += product.PSellingPrice * packageDetail.PdQuantity;
            }

            await dbc.SaveChangesAsync();
            return Ok(new { data = service.SpPackageId });
        }

        [HttpDelete]
        [Route("XoaTblPackage")]
        public async Task<ActionResult> XoaTblService([FromQuery] Guid spPackageId)
        {
            var service = await dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                .FirstOrDefaultAsync(s => s.SpPackageId == spPackageId);

            if (service == null)
            {
                return NotFound("Package not found");
            }

            // Remove table entries (foreign key)
            dbc.TblPackageDetails.RemoveRange(service.TblPackageDetails);
            dbc.TblServicePackages.Remove(service);

            await dbc.SaveChangesAsync();
            return Ok(new { message = "Package deleted successfully" });
        }

        [HttpPost]
        [Route("AddService")]
        public IActionResult AddService(Guid BookingID, [FromBody] List<PackageDto> services)
        {
            foreach (var service in services)
            {
                dbc.Database.ExecuteSqlRaw("EXEC pro_edit_services {0}, {1}, {2}",
                    BookingID, service.PackageId, service.Quantity);
            }
            return NoContent();
        }
        [HttpGet]
        [Route("FindUsedService")]
        public ActionResult FindUsedService(Guid bookingId)
        {
            var services = dbc.TblBookingServices
                .Where(bs => bs.BsBookingId == bookingId)
                .Include(bs => bs.BsService) // Lấy thông tin dịch vụ
                    .ThenInclude(s => s.TblPackageDetails) // Lấy hàng hóa liên quan
                        .ThenInclude(sg => sg.PdProduct) // Lấy thông tin hàng hóa
                .GroupBy(bs => bs.BsServiceId) // Gộp các dịch vụ giống nhau
                .Select(group => new UsedServiceGetDto
                {
                    SpPackageID = group.Key,
                    SpPackageName = group.First().BsService.SpPackageName,
                    SServiceSellPrice = group.First().BsService.TblPackageDetails.Sum(sg => sg.PdProduct.PSellingPrice * sg.PdQuantity),
                    Quantity = group.Sum(bs => bs.BsQuantity), // Tổng số lượng của dịch vụ giống nhau
                    ProductsInfo = string.Join("\n", group.First().BsService.TblPackageDetails
                        .Select(sg => $"{sg.PdQuantity} {sg.PdProduct.PProductName}")),
                    PackageDetails = group.First().BsService.TblPackageDetails.Select(sg => new PackageDetailDto
                    {
                        PdProductId = sg.PdProductId,
                        PdQuantity = sg.PdQuantity
                    }).ToList()
                })
                .ToList();

            return Ok(new { data = services });
        }
    }
}
