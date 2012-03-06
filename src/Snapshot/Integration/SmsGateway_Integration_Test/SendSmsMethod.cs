using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;
using System.Text.RegularExpressions;

namespace IntegrationTests.SmsGateway_Integration_Test
{
    public class SendSmsMethod
    {
        private const string GATEWAY_RESPONSE_REGEX = @"TestMode=1<br>MessageReceived=Please provide current stock level for product group Malaria using format MAL R0<br>Custom={0}+<br>MessageCount=1<br>From=xreplyx<br>CreditsAvailable=[\d, '.']+<br>MessageLength=\d+<br>NumberContacts=1<br>CreditsRequired=[\d, '.']+<br>CreditsRemaining=[\d, '.']+<br>Testmode Active - Nothing Sent";

        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.SetupSmsGatewayService_and_HttpServices();
            objectMother.SetUp_StubData();
            objectMother.Setup_ProductLevelRequest();
        }

        [TearDown]
        public void AfterAll()
        {
            objectMother.Delete_StubData();
        }

        [Test]
        public void Create_An_SmsRequest_And_Successfully_Send_It_Using_The_SmsGatewayService()
        {
            objectMother.queryServiceOutpost.Expect(call => call.Load(objectMother.outpost.Id)).Return(objectMother.outpost);
            objectMother.queryServiceProductGroup.Expect(call => call.Load(objectMother.productGroup.Id)).Return(objectMother.productGroup);
            objectMother.queryServiceStockLevel.Expect(call => call.Query()).Return(objectMother.stockLevels.AsQueryable<OutpostStockLevel>());

            var smsRequest = objectMother.smsRequestService.CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(objectMother.outpost.Id, objectMother.productGroup.Id, objectMother.client);

            string response = objectMother.smsGatewayService.SendSmsRequest(smsRequest).Replace('\n', ' ');

            Assert.IsNotNullOrEmpty(response);

            var str = string.Format(GATEWAY_RESPONSE_REGEX, smsRequest.Id);
            Regex myRegex = new Regex(str, RegexOptions.None);

            var matches = myRegex.Matches(response);

            Assert.AreEqual(1, matches.Count);
        }
    }
}
