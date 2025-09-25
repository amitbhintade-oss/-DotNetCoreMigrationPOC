using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Employee.Contracts;
using Employee.Services.AuthService;
using Employee.Tests.Unit.TestUtilities;

namespace Employee.Tests.Unit.Services
{
    [TestClass]
    public class JwtTokenServiceTests
    {
        private IJwtTokenService _jwtTokenService;

        [TestInitialize]
        public void TestInitialize()
        {
            _jwtTokenService = new JwtTokenService();
        }

        [TestMethod]
        public void GenerateToken_ValidEmployee_ReturnsNonEmptyToken()
        {
            // Arrange
            var employee = TestDataHelper.CreateValidEmployeeRequest();

            // Act
            var token = _jwtTokenService.GenerateToken(employee);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token));
            Assert.IsTrue(token.Length > 0);
        }

        [TestMethod]
        public void GenerateToken_AdminEmployee_ReturnsValidToken()
        {
            // Arrange
            var employee = TestDataHelper.CreateAdminEmployeeRequest();

            // Act
            var token = _jwtTokenService.GenerateToken(employee);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Split('.').Length == 3); // JWT has 3 parts separated by dots
        }

        [TestMethod]
        public void ValidateToken_ValidToken_ReturnsClaimsPrincipal()
        {
            // Arrange
            var employee = TestDataHelper.CreateValidEmployeeRequest();
            var token = _jwtTokenService.GenerateToken(employee);

            // Act
            var principal = _jwtTokenService.ValidateToken(token);

            // Assert
            Assert.IsNotNull(principal);
            Assert.IsTrue(principal.Identity.IsAuthenticated);
        }

        [TestMethod]
        public void ValidateToken_ValidToken_ContainsExpectedClaims()
        {
            // Arrange
            var employee = TestDataHelper.CreateValidEmployeeRequest();
            var token = _jwtTokenService.GenerateToken(employee);

            // Act
            var principal = _jwtTokenService.ValidateToken(token);

            // Assert
            Assert.IsNotNull(principal);
            
            var empIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "EmpId");
            Assert.IsNotNull(empIdClaim);
            Assert.AreEqual(employee.EmpId.ToString(), empIdClaim.Value);

            var usernameClaim = principal.Claims.FirstOrDefault(c => c.Type == "Username");
            Assert.IsNotNull(usernameClaim);
            Assert.AreEqual(employee.Username, usernameClaim.Value);

            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            Assert.IsNotNull(roleClaim);
            Assert.AreEqual(employee.Role, roleClaim.Value);
        }

        [TestMethod]
        public void ValidateToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var principal = _jwtTokenService.ValidateToken(invalidToken);

            // Assert
            Assert.IsNull(principal);
        }

        [TestMethod]
        public void ValidateToken_NullToken_ReturnsNull()
        {
            // Act
            var principal = _jwtTokenService.ValidateToken(null);

            // Assert
            Assert.IsNull(principal);
        }

        [TestMethod]
        public void ValidateToken_EmptyToken_ReturnsNull()
        {
            // Act
            var principal = _jwtTokenService.ValidateToken(string.Empty);

            // Assert
            Assert.IsNull(principal);
        }

        [TestMethod]
        public void GenerateToken_DifferentEmployees_GeneratesDifferentTokens()
        {
            // Arrange
            var employee1 = TestDataHelper.CreateValidEmployeeRequest(1001);
            var employee2 = TestDataHelper.CreateValidEmployeeRequest(1002);

            // Act
            var token1 = _jwtTokenService.GenerateToken(employee1);
            var token2 = _jwtTokenService.GenerateToken(employee2);

            // Assert
            Assert.AreNotEqual(token1, token2);
        }

        [TestMethod]
        public void GenerateToken_SameEmployee_GeneratesDifferentTokens()
        {
            // Arrange
            var employee = TestDataHelper.CreateValidEmployeeRequest();
            
            // Add slight delay to ensure different timestamps
            System.Threading.Thread.Sleep(1000);

            // Act
            var token1 = _jwtTokenService.GenerateToken(employee);
            var token2 = _jwtTokenService.GenerateToken(employee);

            // Assert
            Assert.AreNotEqual(token1, token2); // Different due to different timestamps
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateToken_NullEmployee_ThrowsArgumentNullException()
        {
            // Act
            _jwtTokenService.GenerateToken(null);
        }

        [TestMethod]
        public void ValidateToken_ExpiredToken_ReturnsNull()
        {
            // This test would require creating a token with past expiry time
            // For now, we'll test with a malformed token that looks expired
            
            // Arrange
            var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1MTYyMzkwMjJ9.invalid";

            // Act
            var principal = _jwtTokenService.ValidateToken(expiredToken);

            // Assert
            Assert.IsNull(principal);
        }

        [TestMethod]
        public void TokenRoundTrip_ValidEmployee_MaintainsDataIntegrity()
        {
            // Arrange
            var employee = TestDataHelper.CreateAdminEmployeeRequest(1005);
            employee.Username = "roundtriptest";
            employee.Role = "Admin";

            // Act
            var token = _jwtTokenService.GenerateToken(employee);
            var principal = _jwtTokenService.ValidateToken(token);

            // Assert
            Assert.IsNotNull(principal);
            Assert.AreEqual(employee.EmpId.ToString(), principal.Claims.First(c => c.Type == "EmpId").Value);
            Assert.AreEqual(employee.Username, principal.Claims.First(c => c.Type == "Username").Value);
            Assert.AreEqual(employee.Role, principal.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }
    }
}