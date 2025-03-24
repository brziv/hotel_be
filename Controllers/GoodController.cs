using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public GoodController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetGoodList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblProducts.ToList() });
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
        public async Task<ActionResult> Sua([FromBody] TblProduct updatedProduct)
        {
            var existingProduct = await dbc.TblProducts.FindAsync(updatedProduct.PProductId);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Good not found" });
            }

            existingProduct.PProductId = updatedProduct.PProductId;
            existingProduct.PCategory = updatedProduct.PCategory;
            existingProduct.PQuantity = updatedProduct.PQuantity;
            existingProduct.PUnit = updatedProduct.PUnit;
            existingProduct.PCostPrice = updatedProduct.PCostPrice;
            existingProduct.PSellingPrice = updatedProduct.PSellingPrice;
            existingProduct.PCurrency = updatedProduct.PCurrency;

            dbc.TblProducts.Update(existingProduct);
            dbc.SaveChanges();

            return Ok(new { data = existingProduct });
        }

        [HttpDelete]
        [Route("XoaTblProduct")]
        public async Task<ActionResult> Xoa([FromQuery] Guid PProductId)
        {
            var good = await dbc.TblProducts
                .Include(g => g.TblImportGoodsDetails)
                .FirstOrDefaultAsync(g => g.PProductId == PProductId);

            if (good == null)
            {
                return NotFound("Good not found");
            }

            // Remove TblServiceGood entries (foreign key)
            dbc.TblImportGoodsDetails.RemoveRange(good.TblImportGoodsDetails);
            dbc.TblProducts.Remove(good);

            await dbc.SaveChangesAsync();
            return Ok(new { message = "Good deleted successfully" });
        }
    }
}
