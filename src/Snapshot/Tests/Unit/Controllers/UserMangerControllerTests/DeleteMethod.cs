using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Models.Shared;
using Core.Domain;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_UserId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.Controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a userId in order to remove the user."));
        }

        [Test]
        public void Executes_DeleteCommand_WithTheSelectedUser()
        {
            //Arrange
            objectMother.QueryUsers.Expect(call => call.Load(objectMother.User.Id)).Return(objectMother.User);
            objectMother.DeleteCommand.Expect(call => call.Execute(Arg<User>.Matches(p => p.Id == objectMother.User.Id)));

            //Act
            var jsonResult = objectMother.Controller.Delete(objectMother.User.Id);

            //Assert
            objectMother.DeleteCommand.VerifyAllExpectations();
            objectMother.QueryUsers.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Username admin was removed."));
        }
    }
}
