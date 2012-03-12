using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Models.ClientManager;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.ClientManagerControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Client_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new ClientManagerModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a clientId in order to edit the client."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Client_Has_Been_Saved()
        {
            //Arrange
            ClientManagerModel clientInputModel = new ClientManagerModel()
            {
                Id = objectMother.client.Id,
                Name = objectMother.client.Name
            };
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Client>.Matches(p => p.Name == objectMother.client.Name &&
                                                                                          p.Id == objectMother.client.Id
                                                                                   )));
            objectMother.queryClient.Expect(it => it.Query()).Return(new Client[] { }.AsQueryable());
            objectMother.queryClient.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);

            //Act
            var jsonResult = objectMother.controller.Edit(clientInputModel);

            //Assert
            objectMother.saveCommand.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Client Edgard has been saved."));
        }

        [Test]
        public void Return_JSON_With_ErrorMessage_When_TryingTo_SaveClient_WithExistingName()
        {
            //Arrange
            objectMother.queryClient.Expect(it => it.Query()).Return(new Client[] { objectMother.client }.AsQueryable());

            //Act
            var result = objectMother.controller.Edit(new ClientManagerModel { Id = Guid.NewGuid(), Name = objectMother.client.Name });

            //Assert
            var jsonResult = result.Data as JsonActionResponse;

            Assert.AreEqual(jsonResult.Status, "Error");
            Assert.AreEqual(jsonResult.Message, "There already exist a client with name Edgard. Please insert a different name!");

        }
    }
}
