﻿using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using Web.Models.Shared;
using Web.Models.UserManager;

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
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "UserName"
            };
            var pageOfData = objectMother.PageOfUsersData(indexModel);
            objectMother.QueryUsers.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.Controller.GetListOfUsers(indexModel);

            //Assert
            objectMother.QueryUsers.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<UserOutputModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<UserOutputModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Users_With_ShearchValue_And_Order_ByEmail_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Email",
                searchValue = "admin"
            };

            var pageOfData = objectMother.PageOfUsersData(indexModel);
            objectMother.QueryUsers.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.Controller.GetListOfUsers(indexModel);

            //Assert
            objectMother.QueryUsers.VerifyAllExpectations();

            var jsonData = jsonResult.Data as StoreOutputModel<UserOutputModel>;

            Assert.That(jsonData.Items[0].UserName, Is.EqualTo("9admin"));
            Assert.That(jsonData.Items[0].Email, Is.EqualTo("9"+objectMother.User.Email));

        }




    }
}
