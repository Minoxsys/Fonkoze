using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.UserManager;
using Web.Models.Shared;
using Rhino.Mocks;
using Core.Domain;

namespace Tests.Unit.Controllers.UserMangerControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_User_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new UserManagerInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a userId in order to edit the user."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_User_Has_Been_Saved()
        {
            //Arrange
            UserManagerInputModel userInputModel = new UserManagerInputModel()
            {
                Id = objectMother.user.Id,
                ClientId = objectMother.client.Id,
                ClientName = objectMother.client.Name,
                Email = objectMother.user.Email,
                FirstName = objectMother.user.FirstName,
                LastName = objectMother.user.LastName,
                Password = objectMother.user.Password,
                RoleId = objectMother.user.RoleId,
                RoleName = objectMother.user.RoleName,
                UserName = objectMother.user.UserName
            };
            objectMother.saveCommand.Expect(call => call.Execute(Arg<User>.Matches(p => p.UserName == objectMother.user.UserName &&
                                                                                        p.FirstName == objectMother.user.FirstName &&
                                                                                        p.LastName == objectMother.user.LastName &&
                                                                                        p.RoleName == objectMother.user.RoleName &&
                                                                                        p.ClientName == objectMother.user.ClientName &&
                                                                                        p.Id == objectMother.user.Id
                                                                                   )));
            //Act
            var jsonResult = objectMother.controller.Edit(userInputModel);

            //Assert
            objectMother.saveCommand.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Username Ion.Pop has been saved."));
        }
    }
}
