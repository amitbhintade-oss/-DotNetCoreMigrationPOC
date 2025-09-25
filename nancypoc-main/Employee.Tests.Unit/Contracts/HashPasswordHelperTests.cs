using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Employee.Contracts;

namespace Employee.Tests.Unit.Contracts
{
    [TestClass]
    public class HashPasswordHelperTests
    {
        [TestMethod]
        public void HashPassword_ValidPassword_ReturnsNonEmptyHash()
        {
            // Arrange
            var password = "testpassword123";

            // Act
            var hash = HashPasswordHelper.HashPassword(password);

            // Assert
            Assert.IsNotNull(hash);
            Assert.IsFalse(string.IsNullOrEmpty(hash));
            Assert.IsTrue(hash.Length > 0);
        }

        [TestMethod]
        public void HashPassword_SamePassword_ReturnsSameHash()
        {
            // Arrange
            var password = "testpassword123";

            // Act
            var hash1 = HashPasswordHelper.HashPassword(password);
            var hash2 = HashPasswordHelper.HashPassword(password);

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void HashPassword_DifferentPasswords_ReturnsDifferentHashes()
        {
            // Arrange
            var password1 = "testpassword123";
            var password2 = "testpassword456";

            // Act
            var hash1 = HashPasswordHelper.HashPassword(password1);
            var hash2 = HashPasswordHelper.HashPassword(password2);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HashPassword_NullPassword_ThrowsArgumentException()
        {
            // Act
            HashPasswordHelper.HashPassword(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HashPassword_EmptyPassword_ThrowsArgumentException()
        {
            // Act
            HashPasswordHelper.HashPassword(string.Empty);
        }

        [TestMethod]
        public void ValidatePassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "testpassword123";
            var hash = HashPasswordHelper.HashPassword(password);

            // Act
            var result = HashPasswordHelper.ValidatePassword(password, hash);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidatePassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var correctPassword = "testpassword123";
            var incorrectPassword = "wrongpassword";
            var hash = HashPasswordHelper.HashPassword(correctPassword);

            // Act
            var result = HashPasswordHelper.ValidatePassword(incorrectPassword, hash);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatePassword_NullPassword_ReturnsFalse()
        {
            // Arrange
            var hash = HashPasswordHelper.HashPassword("testpassword123");

            // Act
            var result = HashPasswordHelper.ValidatePassword(null, hash);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatePassword_NullHash_ReturnsFalse()
        {
            // Act
            var result = HashPasswordHelper.ValidatePassword("testpassword123", null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatePassword_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            var hash = HashPasswordHelper.HashPassword("testpassword123");

            // Act
            var result = HashPasswordHelper.ValidatePassword(string.Empty, hash);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatePassword_EmptyHash_ReturnsFalse()
        {
            // Act
            var result = HashPasswordHelper.ValidatePassword("testpassword123", string.Empty);

            // Assert
            Assert.IsFalse(result);
        }
    }
}