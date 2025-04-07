using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportGoodController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public ImportGoodController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetImportGoodList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblImportGoods.ToList() });
        }

        [HttpPost]
        [Route("InsertTblImportGood")]
        public async Task<ActionResult> Them(TblImportGood importGood)
        {
            try
            {
                dbc.TblImportGoods.Add(importGood);
                await dbc.SaveChangesAsync();
                return Ok(new { data = importGood });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
