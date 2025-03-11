using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        DBCnhom4 dbc;
        public EmployeeController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetEmployeeList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblEmployees.ToList() });
        }

        [HttpGet]
        [Route("SearchTblEmployee")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblEmployees
                .Where(item =>
                    item.EFirstName.ToLower().Contains(s) ||
                    item.ELastName.ToLower().Contains(s) ||
                    item.EEmail.ToLower().Contains(s) ||
                    (item.EPhoneNumber != null && item.EPhoneNumber.ToLower().Contains(s)) ||
                    item.EPosition.ToLower().Contains(s) ||
                    item.ESalary.ToString().Contains(s)
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblEmployee")]
        public ActionResult Them(TblEmployee employee)
        {
            dbc.TblEmployees.Add(employee);
            dbc.SaveChanges();

            return Ok(new { data = employee });
        }

        [HttpPut]
        [Route("UpdateTblEmployee")]
        public ActionResult Update(Guid eEmployeeId, string eFirstName, string eLastName, string eEmail, string? ePhoneNumber, string ePosition, decimal eSalary)
        {
            TblEmployee employee = new TblEmployee
            {
                EEmployeeId = eEmployeeId,
                EFirstName = eFirstName,
                ELastName = eLastName,
                EEmail = eEmail,
                EPhoneNumber = ePhoneNumber,
                EPosition = ePosition,
                ESalary = eSalary
            };
            dbc.TblEmployees.Update(employee);
            dbc.SaveChanges();
            return Ok(new { data = employee });
        }

        [HttpDelete]
        [Route("XoaTblEmployee")]
        public ActionResult Xoa(Guid eEmployeeId)
        {
            TblEmployee employee = new TblEmployee
            {
                EEmployeeId = eEmployeeId
            };

            dbc.TblEmployees.Remove(employee);
            dbc.SaveChanges();
            return Ok(new { data = employee });
        }
    }
}
