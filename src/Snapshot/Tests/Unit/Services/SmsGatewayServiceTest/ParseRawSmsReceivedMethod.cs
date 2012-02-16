using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Web.Services;

namespace Tests.Unit.Services.SmsGatewayServiceTest
{
    [TestFixture]
    public class ParseRawSmsReceivedMethod
    {
        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.SetupSmsService_and_MockServices();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void Parses_The_Received_SMS_And_Returns_A_SmsReceived_Entity()
        {
            // Arrange

            // Act
            RawSmsReceivedParseResult parseResult = objectMother.smsGatewayService.ParseRawSmsReceived(objectMother.rawSmsReceived);

            // Assert
            Assert.IsNotNull(parseResult);
            Assert.IsNotNull(parseResult.SmsReceived);
            Assert.AreEqual(objectMother.rawSmsReceivedId, parseResult.SmsReceived.RawSmsReceivedId);
            Assert.AreEqual(ObjectMother.NUMBER, parseResult.SmsReceived.Number);
            Assert.AreEqual("MAL", parseResult.SmsReceived.ProductGroupReferenceCode);
            Assert.IsNotNull(parseResult.SmsReceived.ReceivedStockLevels);
            Assert.AreEqual(2, parseResult.SmsReceived.ReceivedStockLevels.Count);
            Assert.AreEqual("R", parseResult.SmsReceived.ReceivedStockLevels[0].ProductSmsReference);
            Assert.AreEqual(1, parseResult.SmsReceived.ReceivedStockLevels[0].StockLevel);
            Assert.AreEqual("J", parseResult.SmsReceived.ReceivedStockLevels[1].ProductSmsReference);
            Assert.AreEqual(2, parseResult.SmsReceived.ReceivedStockLevels[1].StockLevel);
        }

        [Test]
        public void Parses_The_Received_SMS_And_Returns_Nothing()
        {
            // Arrange

            // Act
            RawSmsReceivedParseResult parseResult= objectMother.smsGatewayService.ParseRawSmsReceived(objectMother.rawSmsReceivedWithBadContent);

            // Assert
            Assert.IsNotNull(parseResult);
            Assert.IsFalse(parseResult.ParseSucceeded);
            Assert.IsNotNullOrEmpty(parseResult.ParseErrorMessage);
            Assert.IsNull(parseResult.SmsReceived);
        }
    }
}
