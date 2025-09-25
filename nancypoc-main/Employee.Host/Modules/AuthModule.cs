using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Employee.Contracts;
using Employee.Services.Service;
using Employee.Services.AuthService;
using Employee.Services.DapperBase;

namespace Employee.Host.Modules
{
    public class AuthModule : NancyModule
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthModule(IEmployeeService employeeService, IJwtTokenService jwtTokenService)
        {
            _employeeService = employeeService;
            _jwtTokenService = jwtTokenService;

            Post("/auth/login", async parameters => await Login());
        }

        private async Task<Response> Login()
        {
            try
            {
                var loginRequest = this.Bind<LoginRequest>();
                
                var employee = await _employeeService.GetEmployeeByIdAsync(loginRequest.EmpId);
                if (employee == null)
                {
                    return Response.AsJson(new { message = "Invalid credentials" }, HttpStatusCode.Unauthorized);
                }

                const string passwordQuery = "SELECT PasswordHash FROM dbo.Employees WHERE EmpId = @EmpId";
                var dapperBase = new Employee.Services.DapperBase.DapperBase(Employee.Services.DbConnectionFactory.GetConnectionString());
                var storedPasswordHash = await dapperBase.QuerySingleOrDefaultPublicAsync<string>(passwordQuery, new { EmpId = loginRequest.EmpId });

                if (string.IsNullOrEmpty(storedPasswordHash) || !HashPasswordHelper.ValidatePassword(loginRequest.Password, storedPasswordHash))
                {
                    return Response.AsJson(new { message = "Invalid credentials" }, HttpStatusCode.Unauthorized);
                }

                var token = _jwtTokenService.GenerateToken(employee);

                return Response.AsJson(new
                {
                    token = token,
                    employee = new
                    {
                        empId = employee.EmpId,
                        username = employee.Username,
                        email = employee.Email,
                        role = employee.Role
                    }
                }, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Response.AsJson(new { message = "An error occurred during login" }, HttpStatusCode.InternalServerError);
            }
        }
    }
}