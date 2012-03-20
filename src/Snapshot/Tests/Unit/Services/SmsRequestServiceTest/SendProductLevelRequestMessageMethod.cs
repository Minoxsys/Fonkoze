using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.SmsRequestServiceTest
{
    [TestFixture]
    class SendProductLevelRequestMessageMethod
    {
        private const string SMS_MESSAGE_TEMPLATE = "Please provide current stock level for product group {0} using format\n{1}";
        private ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            _.Setup_SmsRequestService_And_MockServices();
            _.Setup_Stub_Data();
            _.Setup_ProductLevelRequest();
            _.Setup_ProductLevelRequestMessageInput();
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_The_Method_Will_Generate_An_SmsRequest()
        {
            // Arrange
            _.saveCommandSmsRequest.Expect(it => it.Execute(Arg<SmsRequest>.Matches(
                r => r.ProductGroupId.Equals(_.productGroupId) &&
                    r.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                     r.OutpostId.Equals(_.outpostId) &&
                     r.Message == ObjectMother.MESSAGE_NOT_DELIVERED &&
                     r.Number.Equals(ObjectMother.PHONE_NUMBER) &&
                     r.Client == _.client
                )));

            _.saveCommandSmsRequest.Expect(it => it.Execute(Arg<SmsRequest>.Matches(
                r => r.ProductGroupId.Equals(_.productGroupId) &&
                    r.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                     r.OutpostId.Equals(_.outpostId) &&
                     r.Number.Equals(ObjectMother.PHONE_NUMBER) &&
                     r.Client == _.client &&
                     r.Message.Equals(string.Format(SMS_MESSAGE_TEMPLATE, ObjectMother.PRODUCT_GROUP_NAME, ObjectMother.PRODUCT_GROUP_REFERENCE_CODE + " A0"))
                )));

            _.smsGatewayService.Expect(call => call.SendSmsRequest(Arg<SmsRequest>.Matches(
                r => r.ProductGroupId.Equals(_.productGroupId) &&
                    r.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                     r.OutpostId.Equals(_.outpostId) &&
                     r.Number.Equals(ObjectMother.PHONE_NUMBER) &&
                     r.Client == _.client &&
                     r.Message.Equals(string.Format(SMS_MESSAGE_TEMPLATE, ObjectMother.PRODUCT_GROUP_NAME, ObjectMother.PRODUCT_GROUP_REFERENCE_CODE + " A0"))
                ))).Return("");

            // Act
            var sent = _.smsRequestService.SendProductLevelRequestMessage(_.productLevelRequestMessageInput);

            // Assert
            _.saveCommandSmsRequest.VerifyAllExpectations();
            _.smsGatewayService.VerifyAllExpectations();
            Assert.IsTrue(sent);
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_With_No_Products_The_Method_Will_Generate_Nothing()
        {
            // Arrange

            // Act
            var sent = _.smsRequestService.SendProductLevelRequestMessage(_.productLevelRequestMessageInputWithoutProducts);

            // Assert
            _.saveCommandSmsRequest.AssertWasNotCalled(call => call.Execute(Arg<SmsRequest>.Is.Anything));
            _.smsGatewayService.AssertWasNotCalled(call => call.SendSmsRequest(Arg<SmsRequest>.Is.Anything));
            Assert.IsFalse(sent);
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_With_ContatType_Different_Than_Mobile_Number_The_Method_Will_Generate_Nothing()
        {
            // Arrange

            // Act
            var sent = _.smsRequestService.SendProductLevelRequestMessage(_.productLevelRequestMessageInputWithoutMobileNumber);

            // Assert
            _.saveCommandSmsRequest.AssertWasNotCalled(call => call.Execute(Arg<SmsRequest>.Is.Anything));
            _.smsGatewayService.AssertWasNotCalled(call => call.SendSmsRequest(Arg<SmsRequest>.Is.Anything));
            Assert.IsFalse(sent);
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_With_No_Contact_The_Method_Will_Generate_Nothing()
        {

            // Arrange

            // Act
            var sent = _.smsRequestService.SendProductLevelRequestMessage(_.ProductLevelRequestMessageInputWithoutContact());

            // Assert
            _.saveCommandSmsRequest.AssertWasNotCalled(call => call.Execute(Arg<SmsRequest>.Is.Anything));
            _.smsGatewayService.AssertWasNotCalled(call => call.SendSmsRequest(Arg<SmsRequest>.Is.Anything));
            Assert.IsFalse(sent);
        }
    }
}
