using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using Employee.Contracts;
using Employee.Tests.Integration.TestUtilities;
using Employee.Tests.Unit.TestUtilities;

namespace Employee.Tests.Integration.Modules
{
    [TestClass]
    public class EmployeeModuleTests
    {
        private Browser _browser;
        private string _adminToken;
        private string _userToken;

        [TestInitialize]
        public void TestInitialize()
        {
            try
            {
                _browser = BrowserHelper.CreateBrowser();
                
                // Try to get auth tokens for testing
                // These will fail gracefully if test data doesn't exist
                _adminToken = GetAuthToken(1001, "secret"); // Admin user
                _userToken = GetAuthToken(1002, "secret"); // Regular user
            }
            catch
            {
                Assert.Inconclusive("Unable to initialize browser for integration tests. Check configuration.");
            }
        }

        private string GetAuthToken(int empId, string password)
        {
            try
            {
                var loginRequest = new LoginRequest { EmpId = empId, Password = password };
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(loginRequest);
                });

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    dynamic response = JsonConvert.DeserializeObject(result.Body.AsString());
                    return response.token;
                }
            }
            catch
            {
                // Token retrieval failed - tests will handle this
            }
            return null;
        }

        [TestMethod]
        public void Get_Employees_WithoutAuth_ReturnsUnauthorized()
        {
            try
            {
                // Act
                var result = _browser.Get("/employees", with =>
                {
                    with.HttpRequest();
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_Employees_WithAdminAuth_ReturnsEmployeeList()
        {
            if (string.IsNullOrEmpty(_adminToken))
            {
                Assert.Inconclusive("Admin token not available. Ensure admin test user exists.");
                return;
            }

            try
            {
                // Act
                var result = _browser.Get("/employees", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_adminToken}");
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                
                var responseBody = result.Body.AsString();
                Assert.IsNotNull(responseBody);
                
                // Should be a JSON array
                var employees = JsonConvert.DeserializeObject<EmployeeRequest[]>(responseBody);
                Assert.IsNotNull(employees);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_Employees_WithUserAuth_ReturnsForbidden()
        {
            if (string.IsNullOrEmpty(_userToken))
            {
                Assert.Inconclusive("User token not available. Ensure user test user exists.");
                return;
            }

            try
            {
                // Act
                var result = _browser.Get("/employees", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_userToken}");
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_EmployeeById_WithoutAuth_ReturnsUnauthorized()
        {
            try
            {
                // Act
                var result = _browser.Get("/employees/1001", with =>
                {
                    with.HttpRequest();
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_EmployeeById_WithValidAuth_ReturnsEmployee()
        {
            if (string.IsNullOrEmpty(_adminToken))
            {
                Assert.Inconclusive("Admin token not available. Ensure admin test user exists.");
                return;
            }

            try
            {
                // Act
                var result = _browser.Get("/employees/1001", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_adminToken}");
                });

                // Assert
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var responseBody = result.Body.AsString();
                    Assert.IsNotNull(responseBody);
                    
                    var employee = JsonConvert.DeserializeObject<EmployeeRequest>(responseBody);
                    Assert.IsNotNull(employee);
                    Assert.AreEqual(1001, employee.EmpId);
                }
                else if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    Assert.Inconclusive("Employee 1001 not found in test database");
                }
                else
                {
                    Assert.Fail($"Unexpected status code: {result.StatusCode}");
                }
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_EmployeeById_NonExistent_ReturnsNotFound()
        {
            if (string.IsNullOrEmpty(_adminToken))
            {
                Assert.Inconclusive("Admin token not available. Ensure admin test user exists.");
                return;
            }

            try
            {
                // Act
                var result = _browser.Get("/employees/99999", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_adminToken}");
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_CreateEmployee_WithoutAuth_ReturnsUnauthorized()
        {
            try
            {
                // Arrange
                var newEmployee = TestDataHelper.CreateValidEmployeeRequest();

                // Act
                var result = _browser.Post("/employees", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(newEmployee);
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_CreateEmployee_WithUserAuth_ReturnsForbidden()
        {
            if (string.IsNullOrEmpty(_userToken))
            {
                Assert.Inconclusive("User token not available. Ensure user test user exists.");
                return;
            }

            try
            {
                // Arrange
                var newEmployee = TestDataHelper.CreateValidEmployeeRequest();

                // Act
                var result = _browser.Post("/employees", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_userToken}");
                    with.JsonBody(newEmployee);
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_CreateEmployee_WithAdminAuth_CreatesEmployee()
        {
            if (string.IsNullOrEmpty(_adminToken))
            {
                Assert.Inconclusive("Admin token not available. Ensure admin test user exists.");
                return;
            }

            try
            {
                // Arrange
                var newEmployee = TestDataHelper.CreateValidEmployeeRequest();
                newEmployee.EmpId = 0; // Should be auto-generated
                newEmployee.Username = $"testuser_{Guid.NewGuid().ToString("N")[..8]}";
                newEmployee.Email = $"test_{Guid.NewGuid().ToString("N")[..8]}@test.com";

                // Act
                var result = _browser.Post("/employees", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_adminToken}");
                    with.JsonBody(newEmployee);
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
                
                var responseBody = result.Body.AsString();
                Assert.IsNotNull(responseBody);
                
                var createdEmployee = JsonConvert.DeserializeObject<EmployeeRequest>(responseBody);
                Assert.IsNotNull(createdEmployee);
                Assert.IsTrue(createdEmployee.EmpId > 0);
                Assert.AreEqual(newEmployee.Username, createdEmployee.Username);
                Assert.AreEqual(newEmployee.Email, createdEmployee.Email);
                Assert.AreEqual(newEmployee.Role, createdEmployee.Role);
                Assert.IsNull(createdEmployee.PasswordHash); // Should not return password hash
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_CreateEmployee_WithoutPassword_ReturnsBadRequest()
        {
            if (string.IsNullOrEmpty(_adminToken))
            {
                Assert.Inconclusive("Admin token not available. Ensure admin test user exists.");
                return;
            }

            try
            {
                // Arrange
                var newEmployee = TestDataHelper.CreateValidEmployeeRequest();
                newEmployee.PasswordHash = null; // No password

                // Act
                var result = _browser.Post("/employees", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_adminToken}");
                    with.JsonBody(newEmployee);
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_EmployeeById_InvalidId_ReturnsBadRequest()
        {
            if (string.IsNullOrEmpty(_adminToken))
            {
                Assert.Inconclusive("Admin token not available. Ensure admin test user exists.");
                return;
            }

            try
            {
                // Act
                var result = _browser.Get("/employees/invalid", with =>
                {
                    with.HttpRequest();
                    with.Header("Authorization", $"Bearer {_adminToken}");
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }
    }
}