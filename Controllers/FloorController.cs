using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        public FloorController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetFloorList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblFloors.ToList() });
        }

        [HttpGet]
        [Route("SearchTblFloor")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblFloors
                .Where(item =>
                    item.FFloor.Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblFloor")]
        public ActionResult Them(TblFloor floor)
        {
            dbc.TblFloors.Add(floor);
            dbc.SaveChanges();

            return Ok(new { data = floor });
        }

        [HttpPut]
        [Route("UpdateTblFloor")]
        public ActionResult Sua(Guid fFloorId, string fFloor)
        {
            TblFloor Floor = new TblFloor
            {
                FFloorId = fFloorId,
                FFloor = fFloor
            };
            dbc.SaveChanges();

            return Ok(new { data = Floor });
        }

        [HttpDelete]
        [Route("DeleteTblFloor")]
        public ActionResult Xoa(Guid fFloorId)
        {
            TblFloor Floor = new TblFloor
            {
                FFloorId = fFloorId,
            };

            dbc.TblFloors.Remove(Floor);
            dbc.SaveChanges();
            return Ok(new { data = Floor });
        }
    }
}
