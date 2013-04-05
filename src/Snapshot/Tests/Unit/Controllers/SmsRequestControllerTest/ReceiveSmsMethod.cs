using Domain;
using Domain.Enums;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.SmsRequestControllerTest
{
    [TestFixture]
    public class ReceiveSmsMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void PerTestSetup()
        {
            objectMother.Setup_Controller_And_Mock_Services();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void Assigns_Outpost_To_RawSmsReceived_And_Saves_Saves_The_SMS_AndParses_It_and_Updates_OutputStockLevels()
        {
            // Arrange
            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER)
                                                                                   )));
            objectMother.smsGatewayService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER)
                                                                                                               ))).Return(objectMother.rawSmsReceived);

            objectMother.smsGatewayService.Expect(call => call.ParseRawSmsReceived(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER)
                                                                                       ))).Return(objectMother.rawSmsReceivedParseResult);

            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER) &&
                     r.OutpostId.Equals(objectMother.outpostId) &&
                     r.ParseSucceeded
                                                                                   )));

            objectMother.smsRequestService.Expect(call => call.UpdateOutpostStockLevelsWithValuesReceivedBySms(objectMother.smsReceived));

            // Act
            ActionResult result = objectMother.controller.ReceiveSms(ObjectMother.MOBILE_NUMBER, ObjectMother.SMS_CONTENT, ObjectMother.IN_NUMBER,
                                                                     ObjectMother.EMAIL, ObjectMother.CREDITS);

            // Assert
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.smsGatewayService.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();
            Assert.IsNull(result);
        }

        [Test]
        public void Tries_To_Parse_A_RawSmsReceived_And_Fails_Updates_The_RawSmsReceived_And_The_Parse_Status()
        {
            // Arrange
            objectMother.smsGatewayService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER)
                                                                                                               ))).Return(objectMother.rawSmsReceived);

            objectMother.smsGatewayService.Expect(call => call.ParseRawSmsReceived(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER)
                                                                                       ))).Return(objectMother.rawSmsReceivedParseResultFailed);

            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                r => r.Content.Equals(ObjectMother.SMS_CONTENT) &&
                     r.Credits.Equals(ObjectMother.CREDITS) &&
                     r.Sender.Equals(ObjectMother.MOBILE_NUMBER) &&
                     r.OutpostId.Equals(objectMother.outpostId) &&
                     r.ParseSucceeded == false
                                                                                   )));

            objectMother.queryServiceOutpost.Stub(s => s.Query()).Return((new List<Outpost> {objectMother.outpost}).AsQueryable());

            objectMother.smsRequestService.Expect(call => call.UpdateOutpostStockLevelsWithValuesReceivedBySms(objectMother.smsReceived));

            // Act
            objectMother.controller.ReceiveSms(ObjectMother.MOBILE_NUMBER, ObjectMother.SMS_CONTENT, ObjectMother.IN_NUMBER, ObjectMother.EMAIL,
                                               ObjectMother.CREDITS);

            // Assert
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.smsGatewayService.VerifyAllExpectations();
        }

        [Test]
        public void ReceiveSms_SavesAnErrorAlert_WhenMessageIsIncorrect()
        {
            // Arrange
            objectMother.smsGatewayService.Stub(s => s.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything))
                        .Return(objectMother.rawSmsReceived);

            objectMother.smsGatewayService.Stub(call => call.ParseRawSmsReceived(Arg<RawSmsReceived>.Is.Anything))
                        .Return(objectMother.rawSmsReceivedParseResultFailed);


            objectMother.queryServiceOutpost.Stub(s => s.Query()).Return((new List<Outpost> {objectMother.outpost}).AsQueryable());


            // Act
            objectMother.controller.ReceiveSms(ObjectMother.MOBILE_NUMBER, ObjectMother.SMS_CONTENT, ObjectMother.IN_NUMBER, ObjectMother.EMAIL,
                                               ObjectMother.CREDITS);

            // Assert
            objectMother.saveAlertCmd.AssertWasCalled(
                cm => cm.Execute(Arg<Alert>.Matches(a => a.AlertType == AlertType.Error &&
                                                         a.LowLevelStock == "-" && a.ProductGroupName == "-" && a.Contact == ObjectMother.MOBILE_NUMBER &&
                                                         a.LastUpdate == null && a.OutpostId == objectMother.outpost.Id &&
                                                         a.OutpostName == objectMother.outpost.Name)));
        }
    }
}
