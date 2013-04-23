using Core.Domain;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.CustomModelBinders;
using Web.Security;

namespace Tests.Unit.ModelBinders
{
    [TestFixture]
    public class UserAndClientIdentityModelBinderTests
    {
        private UserAndClientIdentityModelBinder _sut;
        private Mock<IQueryService<User>> _usersQueryServiceMock;
        private Mock<IQueryService<Client>> _clientQueryServiceMock;
        private Mock<ControllerBase> _controllerBaseMock;
        private Mock<HttpContextBase> _httpContextMock;

        [SetUp]
        public void PerTestSetup()
        {
            _httpContextMock = new Mock<HttpContextBase>();
            _controllerBaseMock = new Mock<ControllerBase>();
            _usersQueryServiceMock = new Mock<IQueryService<User>>();
            _clientQueryServiceMock = new Mock<IQueryService<Client>>();
            _sut = new UserAndClientIdentityModelBinder(_usersQueryServiceMock.Object, _clientQueryServiceMock.Object);
        }

        [Test]
        public void BindModel_ReturnsLoggedInUserAndClientInstances_ForTheLoggedInUser()
        {
            //arange
            _httpContextMock.Setup(h => h.User.Identity.Name).Returns("abc");
           
            var dummyUser = new User {UserName = "abc", ClientId = Guid.NewGuid()};
            _usersQueryServiceMock.Setup(q => q.Query()).Returns(new List<User> {dummyUser}.AsQueryable());
            
            var dummyClientMock = new Mock<Client>();
            dummyClientMock.Setup(c => c.Id).Returns(dummyUser.ClientId);
            _clientQueryServiceMock.Setup(q => q.Load(dummyUser.ClientId)).Returns(dummyClientMock.Object);

            //act
            var result = _sut.BindModel(GetFakedControllerContext(), new ModelBindingContext()) as UserAndClientIdentity;

            //assert
            Assert.NotNull(result);
            Assert.That(result.User.UserName, Is.EqualTo("abc"));
            Assert.That(result.Client, Is.Not.Null);
            Assert.That(result.Client.Id, Is.EqualTo(dummyUser.ClientId));
        }

        private ControllerContext GetFakedControllerContext()
        {
            return new ControllerContext(_httpContextMock.Object, new RouteData(), _controllerBaseMock.Object);
        }
    }
}
