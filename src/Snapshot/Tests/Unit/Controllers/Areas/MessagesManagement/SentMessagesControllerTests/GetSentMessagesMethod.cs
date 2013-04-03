using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models.SentMessages;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.SentMessagesControllerTests
{
    [TestFixture]
    public class GetSentMessagesMethod
    {
        private readonly ObjectMother _objectMother = new ObjectMother();

        [SetUp]
        public void PerTestSetup()
        {
            _objectMother.Init();
        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new SentMessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "PhoneNumber"
            };
            var pageOfData = _objectMother.PageOfData(indexModel);
            _objectMother.QuerySms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _objectMother.Controller.GetSentMessages(indexModel);

            //Assert
            _objectMother.QuerySms.VerifyAllExpectations();

            Assert.IsInstanceOf<SentMessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as SentMessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(50, jsonData.TotalItems);
        }

        [Test]
        public void Returns_Messages_Order_DESC_by_ContentMessage()
        {
            //Arrange
            var indexModel = new SentMessagesIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Message"
            };
            var pageOfData = _objectMother.PageOfData(indexModel);
            _objectMother.QuerySms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _objectMother.Controller.GetSentMessages(indexModel);

            //Assert
            _objectMother.QuerySms.VerifyAllExpectations();

            var jsonData = jsonResult.Data as SentMessageIndexOuputModel;
            Assert.NotNull(jsonData);
            Assert.That(jsonData.Messages[0].Message, Is.EqualTo(ObjectMother.Message+"9"));
        }

        [Test]
        public void Returns_Messages_WhereMessageContentContains_SearchValue()
        {
            //Arrange
            var indexModel = new SentMessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "PhoneNumber",
                searchValue = "8"
            };

            var pageOfData = _objectMother.PageOfData(indexModel);
            _objectMother.QuerySms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _objectMother.Controller.GetSentMessages(indexModel);

            //Assert
            _objectMother.QuerySms.VerifyAllExpectations();

            Assert.IsInstanceOf<SentMessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as SentMessageIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
