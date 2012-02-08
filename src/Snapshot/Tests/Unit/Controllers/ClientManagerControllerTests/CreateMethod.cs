using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Models.ClientManager;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.ClientManagerControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();


        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Client_Has_Been_Saved()
        {
            //Arrange
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Client>.Matches(p => p.Name == objectMother.client.Name)));

            //Act
            var jsonResult = objectMother.controller.Create(new ClientManagerModel() { Name = objectMother.client.Name});

            //Assert
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Client Edgard has been saved."));
        }
    }
}
