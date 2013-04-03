//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using Web.Areas.MessagesManagement.Models;
//using Rhino.Mocks;
//using Web.Areas.MessagesManagement.Models.Messages;

//namespace Tests.Unit.Controllers.Areas.MessagesManagement.SellerControllerTests
//{
//    [TestFixture]
//    public class GetMessagesFromDrugShopMethod
//    {
//        public ObjectMother objectMother = new ObjectMother();

//        [SetUp]
//        public void BeforeAll()
//        {
//            objectMother.Init();
//        }

//        [Test]
//        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
//        {
//            //Arrange
//            var indexModel = new MessagesIndexModel
//            {
//                dir = "ASC",
//                limit = 50,
//                page = 1,
//                start = 0,
//                sort = "Sender"
//            };
//            var pageOfData = objectMother.PageOfData(indexModel);
//            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

//            //Act
//            var jsonResult = objectMother.controller.GetMessagesFromDrugShop(indexModel);

//            //Assert
//            objectMother.queryRawSms.VerifyAllExpectations();

//            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
//            var jsonData = jsonResult.Data as MessageIndexOuputModel;
//            Assert.IsNotNull(jsonData);

//            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
//        }

//        [Test]
//        public void Returns_OnlyMessagesFromDrugShops()
//        {
//            //Arrange
//            var indexModel = new MessagesIndexModel
//            {
//                dir = "ASC",
//                limit = 50,
//                page = 1,
//                start = 0,
//                sort = "Sender"
//            };
//            var pageOfData = objectMother.PageOfDrugShopData(indexModel);
//            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

//            //Act
//            var jsonResult = objectMother.controller.GetMessagesFromDrugShop(indexModel);

//            //Assert
//            objectMother.queryRawSms.VerifyAllExpectations();

//            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
//            var jsonData = jsonResult.Data as MessageIndexOuputModel;
//            Assert.IsNotNull(jsonData);

//            Assert.AreEqual(pageOfData.Count() / 2, jsonData.TotalItems);
//        }

//        [Test]
//        public void Returns_OnlyMessagesFromDrugShops_Order_DESC_by_Content()
//        {
//            //Arrange
//            var indexModel = new MessagesIndexModel
//            {
//                dir = "DESC",
//                limit = 50,
//                page = 1,
//                start = 0,
//                sort = "Content"
//            };
//            var pageOfData = objectMother.PageOfDrugShopData(indexModel);
//            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

//            //Act
//            var jsonResult = objectMother.controller.GetMessagesFromDrugShop(indexModel);

//            //Assert
//            objectMother.queryRawSms.VerifyAllExpectations();

//            var jsonData = jsonResult.Data as MessageIndexOuputModel;
//            Assert.That(jsonData.Messages[0].Content, Is.EqualTo(objectMother.rawSms.Content+ "-8"));
//        }
//        [Test]
//        public void Returns_OnlyMessagesFromDrugShops_WhereContentContains_SearchValue()
//        {
//            //Arrange
//            var indexModel = new MessagesIndexModel
//            {
//                dir = "ASC",
//                limit = 50,
//                page = 1,
//                start = 0,
//                sort = "Sender",
//                searchValue = "-8"
//            };

//            var pageOfData = objectMother.PageOfDrugShopData(indexModel);
//            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

//            //Act
//            var jsonResult = objectMother.controller.GetMessagesFromDrugShop(indexModel);

//            //Assert
//            objectMother.queryRawSms.VerifyAllExpectations();

//            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
//            var jsonData = jsonResult.Data as MessageIndexOuputModel;
//            Assert.IsNotNull(jsonData);
//            Assert.AreEqual(1, jsonData.TotalItems);
//        }

//    }
//}
