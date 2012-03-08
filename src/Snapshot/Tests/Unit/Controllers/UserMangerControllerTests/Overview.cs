using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;
using Core.Domain;
using Persistence.Queries.Functions;
namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class Overview
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_The_ViewModel()
        {
            ////Arrange
            //objectMother.queryUsers.Expect(it => it.Query()).Return(new User[] { }.AsQueryable());
            //objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());

            //// Act
            //var viewResult = (ViewResult)objectMother.controller.Overview();

            //// Assert
            //Assert.IsNull(viewResult.Model);
            //Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
