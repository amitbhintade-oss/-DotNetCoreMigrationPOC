using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Employee.Contracts;
using Employee.Tests.Unit.TestUtilities;

namespace Employee.Tests.Unit.Contracts
{
    [TestClass]
    public class LoginRequestTests
    {
        [TestMethod]
        public void LoginRequest_ValidData_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var loginRequest = TestDataHelper.CreateValidLoginRequest();

            // Assert
            Assert.AreEqual(1001, loginRequest.EmpId);
            Assert.AreEqual("testpassword", loginRequest.Password);
        }

        [TestMethod]
        public void LoginRequest_JsonSerialization_SerializesCorrectly()
        {
            // Arrange
            var loginRequest = TestDataHelper.CreateValidLoginRequest();

            // Act
            var json = JsonConvert.SerializeObject(loginRequest);

            // Assert
            Assert.IsTrue(json.Contains("\"EmpId\":1001"));
            Assert.IsTrue(json.Contains("\"Password\":\"testpassword\""));
        }

        [TestMethod]
        public void LoginRequest_JsonDeserialization_DeserializesCorrectly()
        {
            // Arrange
            var json = @"{""EmpId"": 1002, ""Password"": ""jsonpassword""}";

            // Act
            var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(json);

            // Assert
            Assert.IsNotNull(loginRequest);
            Assert.AreEqual(1002, loginRequest.EmpId);
            Assert.AreEqual("jsonpassword", loginRequest.Password);
        }

        [TestMethod]
        public void LoginRequest_DefaultValues_AreSetCorrectly()
        {
            // Act
            var loginRequest = new LoginRequest();

            // Assert
            Assert.AreEqual(0, loginRequest.EmpId);
            Assert.IsNull(loginRequest.Password);
        }

        [TestMethod]
        public void LoginRequest_InvalidData_CanBeCreated()
        {
            // Act
            var loginRequest = TestDataHelper.CreateInvalidLoginRequest();

            // Assert
            Assert.AreEqual(0, loginRequest.EmpId);
            Assert.AreEqual(string.Empty, loginRequest.Password);
        }

        [TestMethod]
        public void LoginRequest_JsonRoundTrip_PreservesData()
        {
            // Arrange
            var originalLoginRequest = TestDataHelper.CreateValidLoginRequest();

            // Act
            var json = JsonConvert.SerializeObject(originalLoginRequest);
            var deserializedLoginRequest = JsonConvert.DeserializeObject<LoginRequest>(json);

            // Assert
            Assert.AreEqual(originalLoginRequest.EmpId, deserializedLoginRequest.EmpId);
            Assert.AreEqual(originalLoginRequest.Password, deserializedLoginRequest.Password);
        }

        [TestMethod]
        public void LoginRequest_DifferentEmpIds_CreatesDifferentInstances()
        {
            // Arrange & Act
            var loginRequest1 = TestDataHelper.CreateValidLoginRequest(1001);
            var loginRequest2 = TestDataHelper.CreateValidLoginRequest(1002);

            // Assert
            Assert.AreNotEqual(loginRequest1.EmpId, loginRequest2.EmpId);
            Assert.AreEqual(loginRequest1.Password, loginRequest2.Password);
        }

        [TestMethod]
        public void LoginRequest_DifferentPasswords_CreatesDifferentInstances()
        {
            // Arrange & Act
            var loginRequest1 = TestDataHelper.CreateValidLoginRequest(1001, "password1");
            var loginRequest2 = TestDataHelper.CreateValidLoginRequest(1001, "password2");

            // Assert
            Assert.AreEqual(loginRequest1.EmpId, loginRequest2.EmpId);
            Assert.AreNotEqual(loginRequest1.Password, loginRequest2.Password);
        }
    }
}