using System.Security.Claims;
using Employee.Contracts;

namespace Employee.Services.AuthService
{
    public interface IJwtTokenService
    {
        string GenerateToken(EmployeeRequest employee);
        ClaimsPrincipal ValidateToken(string token);
    }
}