using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Employee.Contracts;
using Employee.Services.Service;
using Employee.Services.AuthService;
using Employee.Services.DapperBase;

namespace Employee.Host.Modules
{
    [ApiController]
    [Route("auth")]
    public class AuthModule : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthModule(IEmployeeService employeeService, IJwtTokenService jwtTokenService)
        {
            _employeeService = employeeService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
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

            var token = _jwtToken_service.GenerateToken(employee);

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
    }
}
