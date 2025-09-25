using Microsoft.AspNetCore.Mvc;
using Employee.Contracts;
using Employee.Services.Service;
using Employee.Services.AuthService;
using Employee.Services.DapperBase;

namespace MigratedClaudeSonnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IEmployeeService employeeService, IJwtTokenService jwtTokenService)
        {
            _employeeService = employeeService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(loginRequest.EmpId);
                if (employee == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                const string passwordQuery = "SELECT PasswordHash FROM dbo.Employees WHERE EmpId = @EmpId";
                var dapperBase = new Employee.Services.DapperBase.DapperBase(Employee.Services.DbConnectionFactory.GetConnectionString());
                var storedPasswordHash = await dapperBase.QuerySingleOrDefaultPublicAsync<string>(passwordQuery, new { EmpId = loginRequest.EmpId });

                if (string.IsNullOrEmpty(storedPasswordHash) || !HashPasswordHelper.ValidatePassword(loginRequest.Password, storedPasswordHash))
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                var token = _jwtTokenService.GenerateToken(employee);

                return Ok(new
                {
                    token = token,
                    employee = new
                    {
                        empId = employee.EmpId,
                        username = employee.Username,
                        email = employee.Email,
                        role = employee.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }
}