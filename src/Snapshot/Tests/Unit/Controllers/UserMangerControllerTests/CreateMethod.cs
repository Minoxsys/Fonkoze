using Core.Domain;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using Tests.Utils;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        private readonly ObjectMother _objectMother = new ObjectMother();
        

        [SetUp]
        public void BeforeEach()
        {
            _objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_User_Has_Been_Saved()
        {
            //Arrange
            var userInputModel = _objectMother.CreatePopulatedUser();
            _objectMother.SaveCommand.Expect(call => call.Execute(Arg<User>.Matches(p => p.UserName == _objectMother.User.UserName && 
                                                                                        p.FirstName == _objectMother.User.FirstName &&
                                                                                        p.LastName == _objectMother.User.LastName 
                                                                                   )));
            _objectMother.QueryUsers.Stub(s => s.Query()).Return(new List<User>().AsQueryable());

            //Act
            var jsonResult = _objectMother.Controller.Create(userInputModel);

            //Assert
            _objectMother.SaveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Username admin has been created."));
        }

        [Test]
        public void DoNotSaveUser_WhenUserWithSameUsernameAlreadyExists()
        {
            var userModel = _objectMother.CreatePopulatedUser();
            _objectMother.QueryUsers.Stub(s => s.Query()).Return((new List<User> {new User {UserName = userModel.UserName}}).AsQueryable());

            _objectMother.Controller.Create(userModel);

            _objectMother.SaveCommand.AssertWasNotCalled(c => c.Execute(Arg<User>.Is.Anything));
        }

        [Test]
        public void ReturnJSONErrorMessage_WhenUserWithSameUsernameAlreadyExists()
        {
            var userModel = _objectMother.CreatePopulatedUser();
            _objectMother.QueryUsers.Stub(s => s.Query()).Return((new List<User> { new User { UserName = userModel.UserName } }).AsQueryable());

            var result = _objectMother.Controller.Create(userModel);

            Assert.That(result.GetValueFromJsonResult<string>("Status"), Is.EqualTo("Error"));
            Assert.That(result.GetValueFromJsonResult<string>("Message"), Is.EqualTo("Username " + userModel.UserName + " already exists!"));
        }

        [Test]
        public void EncryptPasswordBeforeSaveInDb()
        {
            var userModel = _objectMother.CreatePopulatedUser();
            _objectMother.QueryUsers.Stub(s => s.Query()).Return(new List<User> ().AsQueryable());
            _objectMother.SecurePassword.Stub(s => s.EncryptPassword(userModel.Password)).Return("xxxx");

            _objectMother.Controller.Create(userModel);

            _objectMother.SaveCommand.AssertWasCalled(s => s.Execute(Arg<User>.Matches(u => u.Password == "xxxx")));
        }
    }
}
