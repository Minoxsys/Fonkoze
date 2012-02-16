using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Unit.Services.SmsGatewayServiceTest
{
    [TestFixture]
    public class SendSmsMethod
    {
        private ObjectMother objectMother = new ObjectMother();
        private String postResponse = "Post Response";

        [SetUp]
        public void BeforeAll()
        {
            objectMother.SetupSmsService_and_MockServices();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void SendSMS_Generates_The_Post_String_And_Calls_HttpService_Post()
        {
            // arrange
            objectMother.fakeHttpService.Expect(call => call.Post(ObjectMother.SMS_GATEWAY_URL, objectMother.postRequest)).Return(postResponse);

            // act
            string result = objectMother.smsGatewayService.SendSmsRequest(objectMother.smsRequest);


            //assert
            objectMother.fakeHttpService.VerifyAllExpectations();

            Assert.AreEqual(postResponse, result);
        }
    }
}

