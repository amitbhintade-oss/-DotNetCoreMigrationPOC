using System.Collections.Generic;
using System.Threading.Tasks;
using Employee.Contracts;
using Employee.Services.DapperBase;

namespace Employee.Services.Service
{
    public class EmployeeService : DapperBase.DapperBase, IEmployeeService
    {
        public EmployeeService() : base(DbConnectionFactory.GetConnectionString())
        {
        }

        public async Task<IEnumerable<EmployeeRequest>> GetAllAsync()
        {
            const string sql = "SELECT EmpId, Username, Email, Role, CreatedAt FROM dbo.Employees";
            return await QueryAsync<EmployeeRequest>(sql);
        }

        public async Task<EmployeeRequest> GetEmployeeByIdAsync(int empId)
        {
            const string sql = "SELECT EmpId, Username, Email, Role, CreatedAt FROM dbo.Employees WHERE EmpId = @EmpId";
            return await QuerySingleOrDefaultAsync<EmployeeRequest>(sql, new { EmpId = empId });
        }

        public async Task<int> CreateEmployeeAsync(EmployeeRequest request)
        {
            var hashedPassword = HashPasswordHelper.HashPassword(request.PasswordHash);
            
            const string sql = @"
                INSERT INTO dbo.Employees (Username, PasswordHash, Email, Role) 
                VALUES (@Username, @PasswordHash, @Email, @Role);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await ExecuteScalarAsync<int>(sql, new
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                Email = request.Email,
                Role = request.Role
            });
        }
    }
}