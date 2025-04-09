using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using hotel_be.DTOs;
using Microsoft.AspNetCore.Identity;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DBCnhom4 dbc;
        private readonly UserManager<IdentityUser> _userManager;
        public EmployeeController(DBCnhom4 dbc_in, UserManager<IdentityUser> userManager)
        {
            dbc = dbc_in;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetEmployeeList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblEmployees.ToList() });
        }

        [HttpGet("GetEmployeesWithoutAccounts")]
        public async Task<IActionResult> GetEmployeesWithoutAccounts()
        {
            try
            {
                var employees = await dbc.TblEmployees
                    .Where(e => e.EUserId == null)
                    .Select(e => new
                    {
                        eEmployeeId = e.EEmployeeId,
                        eFirstName = e.EFirstName,
                        eLastName = e.ELastName,
                        eEmail = e.EEmail,
                        ePhoneNumber = e.EPhoneNumber,
                        ePosition = e.EPosition,
                        eSalary = e.ESalary
                    })
                    .ToListAsync();

                return Ok(new { code = 100, data = employees });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, msg = ex.Message });
            }
        }

        [HttpGet("GetEmployeeByUserId")]
        public async Task<IActionResult> GetEmployeeByUserId(Guid userId)
        {
            try
            {
                var employee = await dbc.TblEmployees
                    .Where(e => e.EEmployeeId == userId)
                    .Select(e => new
                    {
                        e.EEmployeeId,
                        e.EFirstName,
                        e.ELastName,
                        e.EEmail,
                        e.EPhoneNumber,
                        e.EPosition,
                        e.ESalary
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                    return NotFound(new { code = 404, msg = "Employee not found" });

                return Ok(new { code = 100, data = employee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, msg = $"Error retrieving employee: {ex.Message}" });
            }
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

        [HttpPut("UpdateEmployeeProfile")]
        public async Task<IActionResult> UpdateTblEmployee([FromBody] UpdateEmployeeDto model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var employee = await dbc.TblEmployees
                    .FirstOrDefaultAsync(e => e.EEmployeeId == model.EEmployeeId);

                if (employee == null)
                {
                    return NotFound(new { code = 404, msg = "Employee not found" });
                }

                employee.EFirstName = model.EFirstName;
                employee.ELastName = model.ELastName;
                employee.EEmail = model.EEmail;
                employee.EPhoneNumber = model.EPhoneNumber;
                employee.EPosition = model.EPosition;
                employee.ESalary = model.ESalary;

                await dbc.SaveChangesAsync();

                return Ok(new { code = 100, msg = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, msg = ex.Message });
            }
        }

        [HttpDelete]
        [Route("XoaTblEmployee")]
        public async Task<ActionResult> Xoa(Guid eEmployeeId)
        {
            try
            {
                // Find the employee including the linked User
                var employee = await dbc.TblEmployees
                    .Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.EEmployeeId == eEmployeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                // Check if there's a linked user account
                if (employee.User != null)
                {
                    // Delete the IdentityUser account
                    var userDeleteResult = await _userManager.DeleteAsync(employee.User);
                    if (!userDeleteResult.Succeeded)
                    {
                        var errors = string.Join("; ", userDeleteResult.Errors.Select(e => e.Description));
                        return BadRequest(new { message = $"Failed to delete user account: {errors}" });
                    }
                }

                // Delete the employee record
                dbc.TblEmployees.Remove(employee);
                await dbc.SaveChangesAsync();

                return Ok(new { data = employee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
