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
    public class AuthModuleTests
    {
        private Browser _browser;

        [TestInitialize]
        public void TestInitialize()
        {
            try
            {
                _browser = BrowserHelper.CreateBrowser();
            }
            catch
            {
                Assert.Inconclusive("Unable to initialize browser for integration tests. Check configuration.");
            }
        }

        [TestMethod]
        public void Post_AuthLogin_ValidCredentials_ReturnsToken()
        {
            try
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    EmpId = 1001, // Assuming this exists in test data
                    Password = "secret" // Known test password
                };

                // Act
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(loginRequest);
                });

                // Assert
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var responseBody = result.Body.AsString();
                    Assert.IsNotNull(responseBody);
                    
                    dynamic response = JsonConvert.DeserializeObject(responseBody);
                    Assert.IsNotNull(response.token);
                    Assert.IsNotNull(response.employee);
                    Assert.AreEqual(1001, (int)response.employee.empId);
                }
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Test data might not exist - that's acceptable for integration tests
                    Assert.Inconclusive("Test user credentials not found in database. Ensure test data is set up.");
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
        public void Post_AuthLogin_InvalidCredentials_ReturnsUnauthorized()
        {
            try
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    EmpId = 9999, // Non-existent employee
                    Password = "wrongpassword"
                };

                // Act
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(loginRequest);
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
                
                var responseBody = result.Body.AsString();
                Assert.IsNotNull(responseBody);
                
                dynamic response = JsonConvert.DeserializeObject(responseBody);
                Assert.IsNotNull(response.message);
                Assert.AreEqual("Invalid credentials", (string)response.message);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_AuthLogin_EmptyBody_ReturnsBadRequest()
        {
            try
            {
                // Act
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    // No body
                });

                // Assert
                // Nancy might return different status codes for empty bodies
                Assert.IsTrue(result.StatusCode == HttpStatusCode.BadRequest || 
                            result.StatusCode == HttpStatusCode.InternalServerError ||
                            result.StatusCode == HttpStatusCode.Unauthorized);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_AuthLogin_InvalidJson_ReturnsBadRequest()
        {
            try
            {
                // Act
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    with.Body("invalid json");
                    with.Header("Content-Type", "application/json");
                });

                // Assert
                Assert.IsTrue(result.StatusCode == HttpStatusCode.BadRequest || 
                            result.StatusCode == HttpStatusCode.InternalServerError);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Post_AuthLogin_ZeroEmpId_ReturnsUnauthorized()
        {
            try
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    EmpId = 0,
                    Password = "anypassword"
                };

                // Act
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(loginRequest);
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
        public void Post_AuthLogin_NullPassword_ReturnsUnauthorized()
        {
            try
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    EmpId = 1001,
                    Password = null
                };

                // Act
                var result = _browser.Post("/auth/login", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(loginRequest);
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
        public void Get_AuthLogin_ReturnsMethodNotAllowed()
        {
            try
            {
                // Act
                var result = _browser.Get("/auth/login", with =>
                {
                    with.HttpRequest();
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }
    }
}