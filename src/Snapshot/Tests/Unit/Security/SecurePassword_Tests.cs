using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Security;
using Core.Services;

namespace Tests.Unit.Security
{
    [TestFixture]
    public class SecurePassword_Tests
    {
        private const string PASSWORD = "password";
        private const string ENCRYPTED_PASSWORD = "mBJU91rk5f8LerUTCrX4GQ==";
        ISecurePassword securePasswordService;

        [Test]
        public void EncryptPassword_Successfully()
        {
            securePasswordService = new SecurePassword();

            string cryptedPassword = securePasswordService.EncryptPassword(PASSWORD);

            Assert.IsNotNullOrEmpty(cryptedPassword);
            Assert.AreEqual(ENCRYPTED_PASSWORD, cryptedPassword);
            
            
        }

        [Test]
        public void DecryptPassword_ReturnsPasswordValueThatWas_BeforeEncrypting()
        {
            securePasswordService = new SecurePassword();
           
            string decryptedPassword = securePasswordService.DecryptPassword(ENCRYPTED_PASSWORD);

            Assert.AreEqual(PASSWORD, decryptedPassword);

 
        }

    }
}
