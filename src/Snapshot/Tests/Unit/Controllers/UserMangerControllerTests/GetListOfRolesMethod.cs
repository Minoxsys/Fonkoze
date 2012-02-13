using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Persistence;
using Core.Domain;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Models.UserManager;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class GetListOfRolesMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Roles()
        {
            //Arange
            objectMother.queryRole.Expect(call => call.Query()).Return(new Role[] {objectMother.role}.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetListOfRoles();

            //Assert
            objectMother.queryRole.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.That(jsonResult, Is.InstanceOfType<JsonResult>());
            Assert.That(jsonResult.Data, Is.InstanceOfType<RolesIndexOutputModel>());
            var jsonData = jsonResult.Data as RolesIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

    }
}
