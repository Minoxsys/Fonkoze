using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Models.Shared;
using Core.Domain;
using Domain;

namespace Tests.Unit.Controllers.ClientManagerControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_ClientId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a clientId in order to remove the client."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_returnedClient_IsNULL()
        {
            //Arrange
            objectMother.queryClient.Expect(call => call.Load(objectMother.client.Id)).Return(null);

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.client.Id);

            //Assert
            objectMother.queryClient.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("There is no client asociated with that id."));
        }
        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Client_Has_Users()
        {
            //Arrange
            objectMother.queryClient.Expect(call => call.Load(objectMother.client.Id)).Return(objectMother.client);
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.client.Id);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("Client Edgard has 1 user(s) associated, and can not be removed."));
        }

        [Test]
        public void Executes_DeleteCommand_WithTheSelectedClient()
        {
            //Arrange
            objectMother.queryClient.Expect(call => call.Load(objectMother.client.Id)).Return(objectMother.client);
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { }.AsQueryable());
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Client>.Matches(p => p.Id == objectMother.client.Id)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.client.Id);

            //Assert
            objectMother.deleteCommand.VerifyAllExpectations();
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Client Edgard was removed."));
        }
    }
}
