using Microsoft.VisualStudio.TestTools.UnitTesting;
using Employee.Contracts;
using Employee.Host.Validators;
using Employee.Tests.Unit.TestUtilities;

namespace Employee.Tests.Unit.Host
{
    [TestClass]
    public class ValidatorTests
    {
        private EmployeeValidator _employeeValidator;
        private LoginValidator _loginValidator;

        [TestInitialize]
        public void TestInitialize()
        {
            _employeeValidator = new EmployeeValidator();
            _loginValidator = new LoginValidator();
        }

        [TestClass]
        public class EmployeeValidatorTests : ValidatorTests
        {
            [TestMethod]
            public void EmployeeValidator_ValidEmployee_PassesValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsTrue(result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
            }

            [TestMethod]
            public void EmployeeValidator_EmptyUsername_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.Username = string.Empty;

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Count > 0);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Username"));
            }

            [TestMethod]
            public void EmployeeValidator_NullUsername_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.Username = null;

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Username"));
            }

            [TestMethod]
            public void EmployeeValidator_EmptyEmail_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.Email = string.Empty;

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Email"));
            }

            [TestMethod]
            public void EmployeeValidator_InvalidEmailFormat_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.Email = "invalid-email-format";

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Email"));
            }

            [TestMethod]
            public void EmployeeValidator_ValidEmailFormat_PassesValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.Email = "valid@example.com";

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsTrue(result.IsValid);
            }

            [TestMethod]
            public void EmployeeValidator_EmptyRole_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.Role = string.Empty;

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Role"));
            }

            [TestMethod]
            public void EmployeeValidator_NewEmployeeWithoutPassword_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.EmpId = 0; // New employee
                employee.PasswordHash = string.Empty;

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "PasswordHash"));
            }

            [TestMethod]
            public void EmployeeValidator_NewEmployeeWithShortPassword_FailsValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.EmpId = 0; // New employee
                employee.PasswordHash = "12345"; // Less than 6 characters

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "PasswordHash"));
            }

            [TestMethod]
            public void EmployeeValidator_ExistingEmployeeWithoutPassword_PassesValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateValidEmployeeRequest();
                employee.EmpId = 1001; // Existing employee
                employee.PasswordHash = null; // Password not required for updates

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsTrue(result.IsValid);
            }

            [TestMethod]
            public void EmployeeValidator_AdminRole_PassesValidation()
            {
                // Arrange
                var employee = TestDataHelper.CreateAdminEmployeeRequest();

                // Act
                var result = _employeeValidator.Validate(employee);

                // Assert
                Assert.IsTrue(result.IsValid);
            }
        }

        [TestClass]
        public class LoginValidatorTests : ValidatorTests
        {
            [TestMethod]
            public void LoginValidator_ValidLogin_PassesValidation()
            {
                // Arrange
                var loginRequest = TestDataHelper.CreateValidLoginRequest();

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsTrue(result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
            }

            [TestMethod]
            public void LoginValidator_ZeroEmpId_FailsValidation()
            {
                // Arrange
                var loginRequest = TestDataHelper.CreateValidLoginRequest();
                loginRequest.EmpId = 0;

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "EmpId"));
            }

            [TestMethod]
            public void LoginValidator_NegativeEmpId_FailsValidation()
            {
                // Arrange
                var loginRequest = TestDataHelper.CreateValidLoginRequest();
                loginRequest.EmpId = -1;

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "EmpId"));
            }

            [TestMethod]
            public void LoginValidator_EmptyPassword_FailsValidation()
            {
                // Arrange
                var loginRequest = TestDataHelper.CreateValidLoginRequest();
                loginRequest.Password = string.Empty;

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Password"));
            }

            [TestMethod]
            public void LoginValidator_NullPassword_FailsValidation()
            {
                // Arrange
                var loginRequest = TestDataHelper.CreateValidLoginRequest();
                loginRequest.Password = null;

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Password"));
            }

            [TestMethod]
            public void LoginValidator_ValidEmpIdAndPassword_PassesValidation()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    EmpId = 1001,
                    Password = "validpassword123"
                };

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsTrue(result.IsValid);
            }

            [TestMethod]
            public void LoginValidator_MultipleErrors_ReportsAllErrors()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    EmpId = 0, // Invalid
                    Password = string.Empty // Invalid
                };

                // Act
                var result = _loginValidator.Validate(loginRequest);

                // Assert
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Count >= 2);
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "EmpId"));
                Assert.IsTrue(result.Errors.Exists(e => e.PropertyName == "Password"));
            }
        }
    }
}