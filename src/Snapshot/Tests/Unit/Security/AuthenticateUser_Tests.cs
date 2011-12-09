using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Core.Persistence;
using Core.Domain;
using Web.Security;
using Rhino.Mocks;
using Core.Services;

namespace Tests.Unit.Security
{
    [TestFixture]
    public class AuthenticateUser_Tests
    {
        IQueryService<User> queryUsers;
        AuthenticateUser authenticateUserService;
        ISecurePassword securePassword;

        User _entity;
        private const string PASSWORD = "password";
        private const string USER_NAME = "alina.ali";
        private Guid USER_ID = new Guid();

        [SetUp]
        public void BeforeAll()
        {
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            securePassword = new SecurePassword();
            authenticateUserService = new AuthenticateUser(queryUsers, securePassword);

            StubEntity();
        }
        public void StubEntity()
        {
            _entity = MockRepository.GeneratePartialMock<User>();
            _entity.Stub(x => x.Id).Return(USER_ID);
            _entity.Password = securePassword.EncryptPassword(PASSWORD);
            _entity.UserName = USER_NAME;

        }
        [Test]
        public void ShouldAuthenticateUser()
        {
            //Arange
            queryUsers.Expect(call => call.Query()).Repeat.Once().Return(new User[] { _entity }.AsQueryable());

            //Act
            bool isAuthenticated = authenticateUserService.ValidateUser(USER_NAME, PASSWORD);

            //Assert
            queryUsers.VerifyAllExpectations();
            Assert.AreEqual(true, isAuthenticated);
        }

        [Test]
        public void ShouldNotAuthenticateUser()
        {
            //Arange
            queryUsers.Expect(call => call.Query()).Repeat.Once().Return(new User[] { }.AsQueryable());

            //Act
            bool isAuthenticated = authenticateUserService.ValidateUser(PASSWORD, USER_NAME);

            //Assert
            queryUsers.VerifyAllExpectations();
            Assert.AreEqual(false, isAuthenticated);
        }
    }
}
