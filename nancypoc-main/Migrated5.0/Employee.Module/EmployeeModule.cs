using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Employee.Contracts;
using Employee.Services.Service;

namespace Employee.Host.Modules
{
    [ApiController]
    [Route("employees")]
    [Authorize]
    public class EmployeeModule : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

n        public EmployeeModule(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

n        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

n        [HttpGet("{id:int}")]
        [Authorize(Policy = "Admin,User")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employee_service.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }
            return Ok(employee);
        }

n        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest employeeRequest)
        {
            if (string.IsNullOrEmpty(employeeRequest.PasswordHash))
            {
                return BadRequest(new { message = "Password is required" });
            }
            var createdEmpId = await _employeeService.CreateEmployeeAsync(employeeRequest);
            employeeRequest.EmpId = createdEmpId;
            employeeRequest.PasswordHash = null;
            return Created(string.Empty, employeeRequest);
        }
    }
}
