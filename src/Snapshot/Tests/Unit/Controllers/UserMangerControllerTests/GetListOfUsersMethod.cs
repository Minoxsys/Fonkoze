using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Models.UserManager;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class GetListOfUsersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new UserManagerIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "UserName"
            };
            var pageOfData = objectMother.PageOfUsersData(indexModel);
            objectMother.queryUsers.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetListOfUsers(indexModel);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();

            Assert.IsInstanceOf<UserIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as UserIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Users_With_ShearchValue_And_Order_ByEmail_DESC()
        {
            //Arrange
            var indexModel = new UserManagerIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Email",
                searchValue = "admin"
            };

            var pageOfData = objectMother.PageOfUsersData(indexModel);
            objectMother.queryUsers.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetListOfUsers(indexModel);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();

            var jsonData = jsonResult.Data as UserIndexOutputModel;

            Assert.That(jsonData.Users[0].UserName, Is.EqualTo("9admin"));
            Assert.That(jsonData.Users[0].Email, Is.EqualTo("9"+objectMother.user.Email));

        }




    }
}
