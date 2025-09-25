using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Employee.Contracts;
using Employee.Tests.Unit.TestUtilities;

namespace Employee.Tests.Unit.Contracts
{
    [TestClass]
    public class EmployeeRequestTests
    {
        [TestMethod]
        public void EmployeeRequest_ValidData_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var employee = TestDataHelper.CreateValidEmployeeRequest();

            // Assert
            Assert.AreEqual(1001, employee.EmpId);
            Assert.AreEqual("testuser", employee.Username);
            Assert.AreEqual("test@company.com", employee.Email);
            Assert.AreEqual("User", employee.Role);
            Assert.IsNotNull(employee.CreatedAt);
            Assert.AreEqual("testpassword", employee.PasswordHash);
        }

        [TestMethod]
        public void EmployeeRequest_JsonSerialization_SerializesCorrectly()
        {
            // Arrange
            var employee = TestDataHelper.CreateValidEmployeeRequest();

            // Act
            var json = JsonConvert.SerializeObject(employee);

            // Assert
            Assert.IsTrue(json.Contains("\"EmpId\":1001"));
            Assert.IsTrue(json.Contains("\"Username\":\"testuser\""));
            Assert.IsTrue(json.Contains("\"Email\":\"test@company.com\""));
            Assert.IsTrue(json.Contains("\"Role\":\"User\""));
            Assert.IsTrue(json.Contains("\"Password\":\"testpassword\""));
        }

        [TestMethod]
        public void EmployeeRequest_JsonDeserialization_DeserializesCorrectly()
        {
            // Arrange
            var json = @"{
                ""EmpId"": 1002,
                ""Username"": ""jsonuser"",
                ""Email"": ""json@company.com"",
                ""Role"": ""Admin"",
                ""Password"": ""jsonpassword"",
                ""CreatedAt"": ""2023-01-01T00:00:00""
            }";

            // Act
            var employee = JsonConvert.DeserializeObject<EmployeeRequest>(json);

            // Assert
            Assert.IsNotNull(employee);
            Assert.AreEqual(1002, employee.EmpId);
            Assert.AreEqual("jsonuser", employee.Username);
            Assert.AreEqual("json@company.com", employee.Email);
            Assert.AreEqual("Admin", employee.Role);
            Assert.AreEqual("jsonpassword", employee.PasswordHash);
            Assert.AreEqual(new DateTime(2023, 1, 1), employee.CreatedAt);
        }

        [TestMethod]
        public void EmployeeRequest_PasswordHashJsonProperty_MapsCorrectly()
        {
            // Arrange
            var json = @"{""Password"": ""secretpassword""}";

            // Act
            var employee = JsonConvert.DeserializeObject<EmployeeRequest>(json);

            // Assert
            Assert.AreEqual("secretpassword", employee.PasswordHash);
        }

        [TestMethod]
        public void EmployeeRequest_DefaultValues_AreSetCorrectly()
        {
            // Act
            var employee = new EmployeeRequest();

            // Assert
            Assert.AreEqual(0, employee.EmpId);
            Assert.IsNull(employee.Username);
            Assert.IsNull(employee.Email);
            Assert.IsNull(employee.Role);
            Assert.AreEqual(default(DateTime), employee.CreatedAt);
            Assert.IsNull(employee.PasswordHash);
        }

        [TestMethod]
        public void EmployeeRequest_AdminRole_SetsCorrectly()
        {
            // Act
            var employee = TestDataHelper.CreateAdminEmployeeRequest();

            // Assert
            Assert.AreEqual("Admin", employee.Role);
        }

        [TestMethod]
        public void EmployeeRequest_JsonRoundTrip_PreservesData()
        {
            // Arrange
            var originalEmployee = TestDataHelper.CreateValidEmployeeRequest();

            // Act
            var json = JsonConvert.SerializeObject(originalEmployee);
            var deserializedEmployee = JsonConvert.DeserializeObject<EmployeeRequest>(json);

            // Assert
            Assert.AreEqual(originalEmployee.EmpId, deserializedEmployee.EmpId);
            Assert.AreEqual(originalEmployee.Username, deserializedEmployee.Username);
            Assert.AreEqual(originalEmployee.Email, deserializedEmployee.Email);
            Assert.AreEqual(originalEmployee.Role, deserializedEmployee.Role);
            Assert.AreEqual(originalEmployee.PasswordHash, deserializedEmployee.PasswordHash);
        }
    }
}