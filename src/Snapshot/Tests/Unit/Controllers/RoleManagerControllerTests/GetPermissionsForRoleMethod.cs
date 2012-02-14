using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Core.Domain;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class GetPermissionsForRoleMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_Permission_Names_For_The_Given_Role_Id()
        {
            // Arrange
            objectMother.queryRole.Expect(call => call.GetPermissions()).Return(new Role[] { objectMother.role }.AsQueryable());

            // Act
            var jsonResult = objectMother.controller.GetPermissionsForRole(objectMother.roleId);

            // Assert
            objectMother.queryServiceRole.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var jsonData = jsonResult.Data as string[];
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(12, jsonData.Count());
        }
    }
}
