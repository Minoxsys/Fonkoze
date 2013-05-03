using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;
using Core.Domain;
using Persistence.Queries.Functions;
using Persistence.Queries.Employees;
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
            objectMother.QueryPermission.Expect(it => it.QueryWithCacheRefresh(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] {}.AsQueryable());
            objectMother.QueryPermission.Expect(it => it.QueryWithCacheRefresh(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { objectMother.Permission }.AsQueryable());
            objectMother.QueryUsers.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { }.AsQueryable());
            objectMother.QueryUsers.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { objectMother.User }.AsQueryable());


            //// Act
            var viewResult = (ViewResult)objectMother.Controller.Overview();

            //// Assert
            Assert.IsNull(viewResult.Model);
            Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
