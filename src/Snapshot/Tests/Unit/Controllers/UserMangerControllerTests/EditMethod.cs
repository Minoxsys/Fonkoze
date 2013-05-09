using Core.Domain;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Shared;
using Web.Models.UserManager;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        private readonly ObjectMother _objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_User_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = _objectMother.Controller.Edit(new UserManagerInputModel());

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
            var userInputModel = new UserManagerInputModel()
                {
                    Id = _objectMother.User.Id,
                    ClientId = _objectMother.Client.Id,
                    Email = _objectMother.User.Email,
                    FirstName = _objectMother.User.FirstName,
                    LastName = _objectMother.User.LastName,
                    Password = _objectMother.User.Password,
                    UserName = _objectMother.User.UserName,
                    RoleId = _objectMother.User.RoleId
                };
            _objectMother.SaveCommand.Expect(call => call.Execute(Arg<User>.Matches(p => p.UserName == _objectMother.User.UserName &&
                                                                                        p.FirstName == _objectMother.User.FirstName &&
                                                                                        p.LastName == _objectMother.User.LastName &&
                                                                                        p.Id == _objectMother.User.Id
                                                                     )));
            _objectMother.QueryUsers.Expect(call => call.Load(_objectMother.UserId)).Return(_objectMother.User);
            //Act
            var jsonResult = _objectMother.Controller.Edit(userInputModel);

            //Assert
            _objectMother.QueryUsers.VerifyAllExpectations();
            _objectMother.SaveCommand.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Username admin has been updated."));
        }

        [Test]
        public void EncryptPasswordBeforeSaveInDb()
        {
            var userModel = _objectMother.CreatePopulatedUser();
            _objectMother.QueryUsers.Stub(s => s.Load(userModel.Id)).Return(new User());
            _objectMother.SecurePassword.Stub(s => s.EncryptPassword(userModel.Password)).Return("xxxx");

            _objectMother.Controller.Edit(userModel);

            _objectMother.SaveCommand.AssertWasCalled(s => s.Execute(Arg<User>.Matches(u => u.Password == "xxxx")));
        }
    }
}
