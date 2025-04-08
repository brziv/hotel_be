using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using hotel_be.DTOs;
using Microsoft.AspNetCore.Authorization;
using hotel_be.ModelFromDB;
using Microsoft.EntityFrameworkCore;
using System;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly DBCnhom4 _dbContext; // Add DbContext for guest/employee operations

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config,
            DBCnhom4 dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _dbContext = dbContext;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    code = 102,
                    msg = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            await _userManager.AddToRoleAsync(user, "User");

            // Create a guest record
            var guest = new TblGuest
            {
                GFirstName = model.FirstName,
                GLastName = model.LastName,
                GPhoneNumber = model.PhoneNumber,
                GUserId = user.Id
            };
            _dbContext.TblGuests.Add(guest);
            await _dbContext.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                code = 100,
                msg = "Registration successful",
                token,
                role = "User",
                userId = guest.GGuestId // Return guest ID for frontend storage
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);

                // Determine if the user is a Guest or Employee and return the appropriate ID
                var guest = await _dbContext.TblGuests
                    .FirstOrDefaultAsync(g => g.GUserId == user.Id);
                var employee = await _dbContext.TblEmployees
                    .FirstOrDefaultAsync(e => e.EUserId == user.Id);

                var userId = guest != null ? guest.GGuestId : employee?.EEmployeeId;

                return Ok(new
                {
                    code = 100,
                    msg = "Login successful",
                    token,
                    role = roles.FirstOrDefault(),
                    userId // Return the linked ID
                });
            }

            return Unauthorized(new
            {
                code = 101,
                msg = "Wrong username or password",
                token = "",
                role = "",
                userId = ""
            });
        }

        [HttpPost("AddStaff")]
        public async Task<IActionResult> AddStaff([FromBody] RegisterStaff model)
        {
            // Validate the employee exists and has no linked account yet
            var employee = await _dbContext.TblEmployees
                .FirstOrDefaultAsync(e => e.EEmployeeId == model.EmployeeId && e.EUserId == null);

            if (employee == null)
            {
                return BadRequest(new
                {
                    code = 103,
                    msg = "Employee not found or already has an account"
                });
            }

            // Create the IdentityUser
            var user = new IdentityUser { UserName = model.Username, Email = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    code = 102,
                    msg = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            // Assign the Staff role
            await _userManager.AddToRoleAsync(user, "Staff");

            // Link the employee to the new user account
            employee.EUserId = user.Id;
            await _dbContext.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                code = 100,
                msg = "Staff account added successfully",
                token,
                role = "Staff",
                userId = employee.EEmployeeId // Return employee ID for frontend storage
            });
        }

        private string GenerateJwtToken(IdentityUser user, IList<string>? roles = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            string? jwtKey = _config["AppSettings:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured in appsettings.json");

            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? throw new ArgumentException("UserName cannot be null")),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (roles != null)
            {
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtHandler.CreateToken(tokenDescriptor);
            return jwtHandler.WriteToken(token);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return NotFound(new { code = 404, msg = "User not found" });
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { code = 400, msg = string.Join("; ", errors) });
                }

                return Ok(new { code = 100, msg = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, msg = ex.Message });
            }
        }
    }
}