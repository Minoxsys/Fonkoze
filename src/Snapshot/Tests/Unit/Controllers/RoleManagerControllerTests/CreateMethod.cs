using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.RoleManager;
using Rhino.Mocks;
using Core.Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Role_Has_Been_Saved()
        {
            //Arrange

            objectMother.queryServicePermission.Expect(call => call.Query()).Return(objectMother.permissions.AsQueryable());
            objectMother.queryServiceRole.Expect(call => call.Query()).Return(new Role[] { objectMother.role2 }.AsQueryable());
            objectMother.saveCommandRole.Expect(call => call.Execute(Arg<Role>.Matches(
                    r => //r.Id == Guid.Empty &&
                    r.Name == ObjectMother.ROLE_NAME &&
                    r.Description == ObjectMother.ROLE_DESCRIPTION &&
                    r.Functions.Count == 6
                    )));

            //Act
            var jsonResult = objectMother.controller.Create(objectMother.inputModel);

            //Assert
            objectMother.saveCommandRole.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Role " + ObjectMother.ROLE_NAME + " has been saved."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleName_Exists()
        {
            //Arrange
            objectMother.queryServiceRole.Expect(call => call.Query()).Return(new Role[] { objectMother.role} .AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(objectMother.inputModel);

            //Assert
            objectMother.queryServiceRole.VerifyAllExpectations();
            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));

        }
    }
}
