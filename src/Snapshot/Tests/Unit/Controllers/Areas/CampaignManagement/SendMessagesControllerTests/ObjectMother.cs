using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistence.Queries.Outposts;
using Web.Services;
using Rhino.Mocks;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.SendMessagesControllerTests
{
    public class ObjectMother
    {
       
        public ISendSmsService smsGatewayService;
        public IQueryService<Outpost> queryOutposts;
        public ISaveOrUpdateCommand<SentSms> SaveOrUpdateCommand;

        public SendMessagesController controller;

        public Outpost outpost;
        public Guid outpostId ;
        public string outpostIds;


        public void SetupSmsService_and_MockServices()
        {
            controller = new SendMessagesController();
            
            queryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
            controller.QueryOutposts = queryOutposts;
            smsGatewayService = MockRepository.GenerateMock<ISendSmsService>();
            controller.smsGatewayService = smsGatewayService;
            SaveOrUpdateCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<SentSms>>();
            controller.SaveOrUpdateCommand = SaveOrUpdateCommand;

            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpostId = Guid.NewGuid();
            outpostIds = outpostId + ",";
            outpost.Stub(o => o.Id).Return(outpostId);
            outpost.DetailMethod = "123456";

        }

    }
}
