using System.Collections.Generic;
using System.Threading.Tasks;
using Employee.Contracts;

namespace Employee.Services.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeRequest>> GetAllAsync();
        Task<EmployeeRequest> GetEmployeeByIdAsync(int empId);
        Task<int> CreateEmployeeAsync(EmployeeRequest request);
    }
}