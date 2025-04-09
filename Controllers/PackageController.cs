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
        [HttpGet]
        [Route("FindServiceRequest")]
        public ActionResult FindServiceRequest()
        {
            // Lấy dữ liệu các dịch vụ đã đặt trong booking đã xác nhận
            var serviceRequests = (from booking in dbc.TblBookings
                                   where booking.BBookingStatus == "Confirmed"
                                   join guest in dbc.TblGuests on booking.BGuestId equals guest.GGuestId
                                   join bookingService in dbc.TblBookingServices on booking.BBookingId equals bookingService.BsBookingId
                                   join servicePackage in dbc.TblServicePackages on bookingService.BsServiceId equals servicePackage.SpPackageId
                                   join bookingRoom in dbc.TblBookingRooms on booking.BBookingId equals bookingRoom.BrBookingId
                                   join room in dbc.TblRooms on bookingRoom.BrRoomId equals room.RRoomId
                                   select new
                                   {
                                       BookingId = booking.BBookingId,
                                       CustomerName = guest.GFirstName + " " + guest.GLastName,
                                       RoomNumber = room.RRoomNumber,
                                       ServiceName = servicePackage.SpPackageName,
                                       Quantity = bookingService.BsQuantity,
                                       CreatedAt = bookingService.BsCreatedAt,
                                       Status = bookingService.BsStatus,
                                       BsId = bookingService.BsId,
                                       ServiceCostPrice = servicePackage.SServiceCostPrice,
                                       ServiceSellPrice = servicePackage.SServiceSellPrice
                                   }).Where(x => x.CreatedAt.HasValue)
                                     .ToList();

            // Group theo CreatedAt đầy đủ (ngày + giờ)
            var result = serviceRequests
                .GroupBy(sr => sr.CreatedAt.Value)
                .Select(group => new
                {
                    CreatedAt = group.Key.ToString("HH:mm:ss - dd/MM/yyyy"),
                    CustomerName = group.First().CustomerName,
                    Room = string.Join("-", group.Select(g => g.RoomNumber).Distinct()),
                    Status = group.First().Status,
                    BookingId = group.First().BookingId,

                    ServiceList = group
                        .GroupBy(g => g.ServiceName)
                        .Select(sg => new
                        {
                            ServiceName = sg.Key,
                            Quantity = sg.Sum(s => s.Quantity),
                            BookingServiceIds = sg.Select(s => s.BsId).ToList(),
                            ServiceCostPrice = sg.First().ServiceCostPrice,
                            ServiceSellPrice = sg.First().ServiceSellPrice,
                            TotalSellPrice = sg.First().ServiceSellPrice * sg.Sum(s => s.Quantity)
                        }).ToList(),

                    TotalAmount = group.Sum(g => g.ServiceSellPrice * g.Quantity)
                })
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            return Ok(new { data = result });
        }


        [HttpPut]
        [Route("ApproveServiceRequest")]
        public ActionResult ApproveServiceRequest([FromBody] ApproveServiceRequestDto request)
        {
            try
            {
                // Tìm Booking theo ID
                var booking = dbc.TblBookings.Find(request.BookingId);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                // Cộng thêm tiền vào BTotalMoney
                booking.BTotalMoney = (booking.BTotalMoney ?? 0) + request.TotalAmount;

                // Lặp qua danh sách BookingServiceIds để cập nhật trạng thái
                foreach (var bsId in request.BookingServiceIds)
                {
                    var bookingService = dbc.TblBookingServices.Find(bsId);
                    if (bookingService != null && bookingService.BsBookingId == request.BookingId)
                    {
                        bookingService.BsStatus = "Confirmed";
                    }
                }

                // Lưu thay đổi vào database
                dbc.SaveChanges();

                return Ok(new { message = "Booking and services updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

    }
}
