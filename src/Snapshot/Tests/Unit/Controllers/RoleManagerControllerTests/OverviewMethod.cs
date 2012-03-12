using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;
using Persistence.Queries.Functions;
using Persistence.Queries.Employees;
using Core.Domain;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class OverviewMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_The_ViewModel()
        {
            //Arrange
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { objectMother.permission }.AsQueryable());
            objectMother.queryServiceUser.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { }.AsQueryable());
            objectMother.queryServiceUser.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { objectMother.user }.AsQueryable());

            // Act
            var viewResult = (ViewResult)objectMother.controller.Overview();

            // Assert
            Assert.IsNull(viewResult.Model);
            Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
