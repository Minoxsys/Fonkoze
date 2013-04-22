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
            
        }
        [Test]
        public void SendSms_Generates_The_Post_String_And_Calls_HttpService_Post()
        {
            // arrange
            objectMother.fakeHttpService.Expect(call => call.Post(ObjectMother.SMS_GATEWAY_URL, "uname=geo_nasa@yahoo.com&pword=asd123&from=xreplyx&test=1&info=0&selectednums=01234567&message=Smscontent")).Return(postResponse);

            // act
            string result = objectMother.smsGatewayService.SendSms("01234567","Smscontent", false);
            
            //assert
            objectMother.fakeHttpService.VerifyAllExpectations();

            Assert.AreEqual(postResponse, result);
        }

    }
}
