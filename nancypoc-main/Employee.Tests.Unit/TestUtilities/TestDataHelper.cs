using System;
using Employee.Contracts;

namespace Employee.Tests.Unit.TestUtilities
{
    public static class TestDataHelper
    {
        public static EmployeeRequest CreateValidEmployeeRequest(int empId = 1001, string role = "User")
        {
            return new EmployeeRequest
            {
                EmpId = empId,
                Username = "testuser",
                Email = "test@company.com",
                Role = role,
                CreatedAt = DateTime.Now,
                PasswordHash = "testpassword"
            };
        }

        public static EmployeeRequest CreateAdminEmployeeRequest(int empId = 1001)
        {
            return CreateValidEmployeeRequest(empId, "Admin");
        }

        public static LoginRequest CreateValidLoginRequest(int empId = 1001, string password = "testpassword")
        {
            return new LoginRequest
            {
                EmpId = empId,
                Password = password
            };
        }

        public static EmployeeRequest CreateInvalidEmployeeRequest()
        {
            return new EmployeeRequest
            {
                EmpId = 0,
                Username = "",
                Email = "invalid-email",
                Role = "",
                PasswordHash = ""
            };
        }

        public static LoginRequest CreateInvalidLoginRequest()
        {
            return new LoginRequest
            {
                EmpId = 0,
                Password = ""
            };
        }

        public const string TestPassword = "testpassword123";
        public const string TestPasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg="; // Hash of "secret"
        public const string ValidJwtSecret = "TestSecretKeyForUnitTestsOnly123456789012345678901234567890";
    }
}