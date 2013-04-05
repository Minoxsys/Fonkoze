using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web.Mvc;
using Persistence.Queries.Functions;
using Core.Domain;
using Persistence.Queries.Employees;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    [TestFixture]
    public class OverviewMethod
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
            //Arrange
            objectMother.queryPermission.Expect(it => it.QueryWithCacheRefresh(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.QueryWithCacheRefresh(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.queryUsers.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { }.AsQueryable());
           
            // Act
            var viewResult = (ViewResult)objectMother.controller.Overview();

            // Assert
            Assert.IsNull(viewResult.Model);
            Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
