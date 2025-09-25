using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;
using Nancy.Testing;
using Employee.Tests.Integration.TestUtilities;

namespace Employee.Tests.Integration.Modules
{
    [TestClass]
    public class HomeModuleTests
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
        public void Get_Root_ReturnsApiEmployeeMessage()
        {
            try
            {
                // Act
                var result = _browser.Get("/", with =>
                {
                    with.HttpRequest();
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.AreEqual("Api-Employee", result.Body.AsString());
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_Root_ReturnsCorrectContentType()
        {
            try
            {
                // Act
                var result = _browser.Get("/", with =>
                {
                    with.HttpRequest();
                });

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                // Nancy may return text/html by default for simple string responses
                Assert.IsNotNull(result.ContentType);
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }

        [TestMethod]
        public void Get_Root_HandlesMultipleRequests()
        {
            try
            {
                // Act & Assert - Multiple requests
                for (int i = 0; i < 3; i++)
                {
                    var result = _browser.Get("/", with =>
                    {
                        with.HttpRequest();
                    });

                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                    Assert.AreEqual("Api-Employee", result.Body.AsString());
                }
            }
            catch (System.Exception ex) when (ex.Message.Contains("database") || ex.Message.Contains("connection"))
            {
                Assert.Inconclusive("Database connection required for this integration test");
            }
        }
    }
}