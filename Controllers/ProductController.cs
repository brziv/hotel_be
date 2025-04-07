using hotel_be.DTOs;
using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public ProductController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetProductList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblProducts.Where(p => p.IsActive).ToList() });
        }

        [HttpPost]
        [Route("InsertTblProduct")]
        public ActionResult Them(TblProduct product)
        {
            dbc.TblProducts.Add(product);
            dbc.SaveChanges();

            return Ok(new { data = product });
        }

        [HttpPut]
        [Route("UpdateTblProduct")]
        public async Task<ActionResult> Sua([FromBody] UpdateProductDto updatedProduct)
        {
            var existingProduct = await dbc.TblProducts
                .Include(p => p.TblPackageDetails)
                .FirstOrDefaultAsync(p => p.PProductId == updatedProduct.PProductId);

            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            bool isPriceChanged =
                existingProduct.PCostPrice != updatedProduct.PCostPrice ||
                existingProduct.PSellingPrice != updatedProduct.PSellingPrice;

            bool isInActivePackages = existingProduct.TblPackageDetails.Any();

            if (isPriceChanged && isInActivePackages)
            {
                // Soft delete the existing product
                existingProduct.IsActive = false;

                // Create a new product
                var newProduct = new TblProduct
                {
                    PProductId = Guid.NewGuid(),
                    PProductName = updatedProduct.PProductName,
                    PCategory = updatedProduct.PCategory,
                    PQuantity = updatedProduct.PQuantity,
                    PUnit = updatedProduct.PUnit,
                    PCostPrice = updatedProduct.PCostPrice,
                    PSellingPrice = updatedProduct.PSellingPrice,
                    PCurrency = updatedProduct.PCurrency,
                    PIsService = updatedProduct.PIsService,
                    IsActive = true
                };

                dbc.TblProducts.Add(newProduct);
                await dbc.SaveChangesAsync();

                return Ok(new { message = "Product duplicated with new prices.", newProduct });
            }

            // Update the existing product if prices haven't changed or not in active packages
            existingProduct.PProductName = updatedProduct.PProductName;
            existingProduct.PCategory = updatedProduct.PCategory;
            existingProduct.PQuantity = updatedProduct.PQuantity;
            existingProduct.PUnit = updatedProduct.PUnit;
            existingProduct.PCostPrice = updatedProduct.PCostPrice;
            existingProduct.PSellingPrice = updatedProduct.PSellingPrice;
            existingProduct.PCurrency = updatedProduct.PCurrency;
            existingProduct.PIsService = updatedProduct.PIsService;
            existingProduct.IsActive = updatedProduct.IsActive;

            await dbc.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully." });
        }

        [HttpDelete]
        [Route("XoaTblProduct")]
        public async Task<ActionResult> Xoa([FromQuery] Guid PProductId)
        {
            var product = await dbc.TblProducts
                .Include(g => g.TblImportGoodsDetails)
                .Include(g => g.TblPackageDetails)
                .FirstOrDefaultAsync(g => g.PProductId == PProductId);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            if (product.PQuantity > 0)
            {
                return BadRequest(new { message = "Cannot delete product with quantity greater than 0" });
            }

            // Check if related records exist
            bool hasImportDetails = product.TblImportGoodsDetails?.Any() == true;
            bool hasPackageDetails = product.TblPackageDetails?.Any() == true;

            if (!hasImportDetails && !hasPackageDetails)
            {
                // Hard delete
                dbc.TblProducts.Remove(product);
                await dbc.SaveChangesAsync();
                return Ok(new { message = "Product permanently deleted" });
            }
            else
            {
                // Soft delete
                product.IsActive = false;
                await dbc.SaveChangesAsync();
                return Ok(new { message = "Product soft deleted successfully" });
            }
        }

    }
}
