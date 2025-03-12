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
        public ActionResult Update([FromBody] TblEmployee updatedEmployee)
        {
            var existingEmployee = dbc.TblEmployees.Find(updatedEmployee.EEmployeeId);
            if (existingEmployee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            existingEmployee.EFirstName = updatedEmployee.EFirstName;
            existingEmployee.ELastName = updatedEmployee.ELastName;
            existingEmployee.EEmail = updatedEmployee.EEmail;
            existingEmployee.EPhoneNumber = updatedEmployee.EPhoneNumber;
            existingEmployee.EPosition = updatedEmployee.EPosition;
            existingEmployee.ESalary = updatedEmployee.ESalary;

            dbc.TblEmployees.Update(existingEmployee);
            dbc.SaveChanges();

            return Ok(new { data = existingEmployee });
        }

        [HttpDelete]
        [Route("XoaTblEmployee")]
        public ActionResult Xoa(Guid eEmployeeId)
        {
            var employee = dbc.TblEmployees.Find(eEmployeeId);

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            dbc.TblEmployees.Remove(employee);
            dbc.SaveChanges();

            return Ok(new { data = employee });
        }
    }
}
