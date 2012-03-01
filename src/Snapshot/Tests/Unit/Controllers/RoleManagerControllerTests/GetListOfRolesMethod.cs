using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Core.Domain;
using System.Web.Mvc;
using Web.Models.RoleManager;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class GetListOfRolesMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Return_JSON_With_List_Of_Roles()
        {
            // Arrange
            objectMother.queryServiceRole.Expect(call => call.Query()).Return(new Role[] { objectMother.role }.AsQueryable());
            objectMother.queryServiceUser.Expect(call => call.Query()).Return(new User[] { }.AsQueryable());

            // Act
            var jsonResult = objectMother.controller.GetListOfRoles(objectMother.indexModel);
            
            // Assert
            objectMother.queryServiceRole.VerifyAllExpectations();
            objectMother.queryServiceUser.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf(typeof(JsonResult), jsonResult);
            Assert.IsInstanceOf(typeof(RoleManagerListForJsonOutput), jsonResult.Data);

            var jsonData = jsonResult.Data as RoleManagerListForJsonOutput;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.Roles.Count());
            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
