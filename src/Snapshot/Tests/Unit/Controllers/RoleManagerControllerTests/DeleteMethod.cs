using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Rhino.Mocks;
using Core.Domain;
using Web.Models.RoleManager;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a roleId in order to remove the role."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleId_Is_Invalid()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(Guid.NewGuid());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply the roleId of a role that exists in the DB in order to remove it."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Role_HasUsersAssociated()
        {
            objectMother.queryServiceRole.Expect(call => call.Load(objectMother.roleId)).Return(objectMother.role);
            objectMother.queryServiceUser.Expect(call => call.Query()).Return(new User[] { new User() { UserName = "Marian", RoleId = objectMother.roleId } }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.roleId);

            //Assert
            objectMother.queryServiceRole.VerifyAllExpectations();
            objectMother.queryServiceUser.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Executes_DeleteCommand_WithTheSelectedUser()
        {
            //Arrange
            objectMother.queryServiceRole.Expect(call => call.Load(objectMother.roleId)).Return(objectMother.role);
            objectMother.deleteCommandRole.Expect(call => call.Execute(Arg<Role>.Matches(r => r.Id == objectMother.roleId)));
            objectMother.queryServiceUser.Expect(call => call.Query()).Return(new User[] {  }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.roleId);

            //Assert
            objectMother.queryServiceRole.VerifyAllExpectations();
            objectMother.deleteCommandRole.VerifyAllExpectations();
            objectMother.queryServiceUser.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Role " + ObjectMother.ROLE_NAME + " was removed."));
        }
    }
}
