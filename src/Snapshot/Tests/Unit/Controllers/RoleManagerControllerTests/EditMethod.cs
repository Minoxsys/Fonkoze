using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Core.Domain;
using Web.Models.Shared;
using Web.Models.RoleManager;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Role_Has_Been_Updated()
        {
            //Arrange

            objectMother.queryServiceRole.Expect(call => call.Load(objectMother.roleId)).Return(objectMother.role);
            objectMother.queryServiceRole.Expect(call => call.Query()).Return(new Role[] { objectMother.role }.AsQueryable());
            objectMother.queryServicePermission.Expect(call => call.Query()).Return(objectMother.permissions.AsQueryable());

            objectMother.saveCommandRole.Expect(call => call.Execute(Arg<Role>.Matches(
                    r => r.Id == objectMother.roleId &&
                    r.Name == ObjectMother.ROLE_NAME &&
                    r.Description == ObjectMother.ROLE_DESCRIPTION &&
                    r.Functions.Count == 6
                    )));

            //Act
            var jsonResult = objectMother.controller.Edit(objectMother.inputModel);

            //Assert
            objectMother.queryServiceRole.VerifyAllExpectations();
            objectMother.saveCommandRole.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Role " + ObjectMother.ROLE_NAME + " has been saved."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleManagerInputModel_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new RoleManagerInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a roleId in order to edit the role."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleManagerInputModel_Has_An_Invalid_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new RoleManagerInputModel() { Id = Guid.NewGuid() });

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply the roleId of a role that exists in the DB in order to edit it."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleName_Exists()
        {
            //Arrange
            objectMother.queryServiceRole.Expect(call => call.Query()).Return(new Role[] { objectMother.role }.AsQueryable());
            objectMother.queryServiceRole.Expect(call => call.Load(objectMother.roleId2)).Return(objectMother.role2);

            //Act
            var jsonResult = objectMother.controller.Edit(new RoleManagerInputModel() { Id = objectMother.roleId2, Name = objectMother.role.Name});

            //Assert
            objectMother.queryServiceRole.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }
    }
}
