using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using Rhino.Mocks;
using Persistence.Queries.Outposts;
using Core.Persistence;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.SendMessagesControllerTests
{

    
        
    [TestFixture]
    public class SendMessageToOutpostsMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();
        private string message = "test";

        [SetUp]
        public void BeforeEach()
        {
            objectMother.SetupSmsService_and_MockServices();

        }

        [Test]
        public void Should_QueryOutpostsAndCallSendSmsAndReturnJsonActionResponse()
        {
            objectMother.queryOutposts.Expect(call => call.Query()).Return(new Outpost[] {objectMother.outpost}.AsQueryable());
            objectMother.smsGatewayService.Expect(call => call.SendSms(objectMother.outpost.DetailMethod, message, true)).Return(null);

            var jsonResult = objectMother.controller.SendMessageToOutposts(objectMother.outpostIds, message);

            objectMother.queryOutposts.VerifyAllExpectations();
            objectMother.smsGatewayService.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<JsonActionResponse>(jsonResult.Data);
            var jsonData = jsonResult.Data as JsonActionResponse;
           
            Assert.AreEqual("Success", jsonData.Status);
        
        }


    }
}
