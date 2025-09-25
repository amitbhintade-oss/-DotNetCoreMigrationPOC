using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Employee.Contracts;
using Employee.Services.Service;

namespace Employee.Host.Modules
{
    public class EmployeeModule : NancyModule
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeModule(IEmployeeService employeeService) : base("/employees")
        {
            _employeeService = employeeService;

            Get("/", async parameters => await GetAllEmployees());
            Get("/{id:int}", async parameters => await GetEmployeeById(parameters.id));
            Post("/", async parameters => await CreateEmployee());
        }

        private async Task<Response> GetAllEmployees()
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { "Admin" });

            try
            {
                var employees = await _employeeService.GetAllAsync();
                return Response.AsJson(employees, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Response.AsJson(new { message = "An error occurred while retrieving employees" }, HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response> GetEmployeeById(int id)
        {
            this.RequiresAuthentication();
            this.RequiresAnyClaim(new[] { "Admin", "User" });

            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return Response.AsJson(new { message = "Employee not found" }, HttpStatusCode.NotFound);
                }

                return Response.AsJson(employee, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Response.AsJson(new { message = "An error occurred while retrieving employee" }, HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response> CreateEmployee()
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { "Admin" });

            try
            {
                var employeeRequest = this.Bind<EmployeeRequest>();
                
                if (string.IsNullOrEmpty(employeeRequest.PasswordHash))
                {
                    return Response.AsJson(new { message = "Password is required" }, HttpStatusCode.BadRequest);
                }

                var createdEmpId = await _employeeService.CreateEmployeeAsync(employeeRequest);
                employeeRequest.EmpId = createdEmpId;
                employeeRequest.PasswordHash = null; // Don't return password hash

                return Response.AsJson(employeeRequest, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Response.AsJson(new { message = "An error occurred while creating employee" }, HttpStatusCode.InternalServerError);
            }
        }
    }
}