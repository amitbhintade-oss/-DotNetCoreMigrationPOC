using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Employee.Contracts;
using Employee.Services.Service;

namespace MigratedClaudeSonnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving employees" });
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving employee" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest employeeRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeRequest.PasswordHash))
                {
                    return BadRequest(new { message = "Password is required" });
                }

                var createdEmpId = await _employeeService.CreateEmployeeAsync(employeeRequest);
                employeeRequest.EmpId = createdEmpId;
                employeeRequest.PasswordHash = null; // Don't return password hash

                return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmpId }, employeeRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating employee" });
            }
        }
    }
}