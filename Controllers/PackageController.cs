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
                .Where(s => s.IsActive)
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
                IsActive = true,
                TblPackageDetails = new List<TblPackageDetail>()
            };

            foreach (var packageDetail in packageDto.PackageDetails)
            {
                var product = await dbc.TblProducts.FindAsync(packageDetail.PdProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"product with ID {packageDetail.PdProductId} not found" });
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

        [HttpDelete]
        [Route("XoaTblPackage")]
        public async Task<ActionResult> XoaTblService([FromQuery] Guid spPackageId)
        {
            var service = await dbc.TblServicePackages
                .Include(s => s.TblPackageDetails)
                .Include(s => s.TblBookingServices)
                .FirstOrDefaultAsync(s => s.SpPackageId == spPackageId);

            if (service == null)
            {
                return NotFound(new { message = "Package not found" });
            }

            // Check if package has been booked
            bool hasBookingServices = service.TblBookingServices?.Any() == true;

            if (!hasBookingServices)
            {
                // Hard delete: remove from the database
                if (service.TblPackageDetails != null && service.TblPackageDetails.Any())
                {
                    dbc.TblPackageDetails.RemoveRange(service.TblPackageDetails);
                }

                dbc.TblServicePackages.Remove(service);
                await dbc.SaveChangesAsync();
                return Ok(new { message = "Package permanently deleted" });
            }
            else
            {
                // Soft delete: mark as inactive
                service.IsActive = false;
                await dbc.SaveChangesAsync();
                return Ok(new { message = "Package soft deleted successfully" });
            }
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
                .Include(bs => bs.BsService)
                    .ThenInclude(s => s.TblPackageDetails)
                        .ThenInclude(sg => sg.PdProduct)
                .GroupBy(bs => bs.BsServiceId)
                .Select(group => new UsedServiceGetDto
                {
                    SpPackageID = group.Key,
                    SpPackageName = group.First().BsService.SpPackageName,
                    SServiceSellPrice = group.First().BsService.TblPackageDetails.Sum(sg => sg.PdProduct.PSellingPrice * sg.PdQuantity),
                    Quantity = group.Sum(bs => bs.BsQuantity),
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
