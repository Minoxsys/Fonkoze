using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using System.Net.Mail;

namespace Tests.Unit.Services.EmailRequestServiceTests
{
    [TestFixture]
    public class SendProductLevelRequestMessageMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            _.Setup_EmailRequestService_and_MockServices();
            _.SetUp_StubData();
            _.Setup_ProductLevelRequestMessageInput();
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_The_Method_Will_Generate_An_SmsRequest()
        {
            // Arrange
            _.saveOrUpdateCommandEmailRequest.Expect(it => it.Execute(Arg<EmailRequest>.Matches(
                c => c.Date.ToShortDateString() == DateTime.UtcNow.ToShortDateString() &&
                    c.OutpostId == _.outpostId &&
                    c.ProductGroupId == _.productGroup.Id &&
                    c.Client == _.client
                )));

            _.emailService.Expect(call => call.SendMail(Arg<MailMessage>.Is.Anything)).Return(true);

            // Act
            bool sent = _.emailRequestService.SendProductLevelRequestMessage(_.productLevelRequestMessageInput);

            // Assert
            _.saveOrUpdateCommandEmailRequest.VerifyAllExpectations();
            _.emailService.VerifyAllExpectations();
            Assert.IsTrue(sent);
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_With_No_Products_The_Method_Will_Generate_Nothing()
        {
            // Arrange

            // Act
            bool sent = _.emailRequestService.SendProductLevelRequestMessage(_.productLevelRequestMessageInputWithoutProducts);

            // Assert
            _.saveOrUpdateCommandEmailRequest.AssertWasNotCalled(call => call.Execute(Arg<EmailRequest>.Matches(r => true)));
            _.emailService.AssertWasNotCalled(call => call.SendMail(Arg<MailMessage>.Matches(r => true)));
            Assert.IsFalse(sent);
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_With_ContatType_Different_Than_Mobile_Number_The_Method_Will_Generate_Nothing()
        {
            // Arrange

            // Act
            bool sent = _.emailRequestService.SendProductLevelRequestMessage(_.productLevelRequestMessageInputWithoutEmail);

            // Assert
            _.saveOrUpdateCommandEmailRequest.AssertWasNotCalled(call => call.Execute(Arg<EmailRequest>.Matches(r => true)));
            _.emailService.AssertWasNotCalled(call => call.SendMail(Arg<MailMessage>.Matches(r => true)));
            Assert.IsFalse(sent);
        }

        [Test]
        public void Given_An_ProductLevelRequestMessageInput_With_No_Contact_The_Method_Will_Generate_Nothing()
        {
            // Arrange

            // Act
            bool sent = _.emailRequestService.SendProductLevelRequestMessage(_.ProductLevelRequestMessageInputWithNullContact());

            // Assert
            _.saveOrUpdateCommandEmailRequest.AssertWasNotCalled(call => call.Execute(Arg<EmailRequest>.Matches(r => true)));
            _.emailService.AssertWasNotCalled(call => call.SendMail(Arg<MailMessage>.Matches(r => true)));
            Assert.IsFalse(sent);
        }
    }
}
