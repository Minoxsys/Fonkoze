using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Domain.Enums;
using Web.Areas.MessagesManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Services;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.SellerControllerTests
{
    public class ObjectMother
    {
        public SellerController Controller;
        public IQueryService<RawSmsReceived> QueryRawSms;
        public IQueryService<Outpost> QueryOutposts;
        private IRawSmsMeesageQueryHelpersService _rawSmsMeesageQueryHelpersService;

        public Guid RawSmsId;
        public RawSmsReceived RawSms;

        private const string Sender = "0747548965";
        private const string Content = "FTT452518G RS!GDD11";
        private readonly DateTime _date = DateTime.UtcNow;
        private const string Credits = "10";
        private readonly Guid _outpostid = Guid.NewGuid();
        private const string Outpostname = "Spitalul Judetean";
        private const string Errormessage = "Not parse correct";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void SetUp_StubData()
        {
            RawSmsId = Guid.NewGuid();
            RawSms = MockRepository.GeneratePartialMock<RawSmsReceived>();
            RawSms.Stub(c => c.Id).Return(RawSmsId);
            RawSms.Content = Content;
            RawSms.OutpostType = 0;
            RawSms.OutpostId = _outpostid;
            RawSms.ParseErrorMessage = Errormessage;
            RawSms.ParseSucceeded = false;
            RawSms.ReceivedDate = _date;
            RawSms.Sender = Sender;
        }

        private void Setup_Controller()
        {
            Controller = new SellerController();
            //Controller.QueryRawSms = QueryRawSms;
            // Controller.QueryOutpost = QueryOutposts;
        }

        private void MockServices()
        {
            QueryRawSms = MockRepository.GenerateMock<IQueryService<RawSmsReceived>>();
            QueryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
            //_rawSmsMeesageQueryHelpersService = MockRepository.GenerateMock<I>()
        }

        public IQueryable<RawSmsReceived> PageOfData(MessagesIndexModel indexModel)
        {
            List<RawSmsReceived> rawSMSList = new List<RawSmsReceived>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                rawSMSList.Add(new RawSmsReceived
                    {
                        Content = Content + "-" + i,
                        OutpostType = 0,
                        OutpostId = Guid.NewGuid(),
                        ParseErrorMessage = "Parse error no." + i,
                        ParseSucceeded = false,
                        ReceivedDate = DateTime.UtcNow.AddDays(-i),
                        Sender = Sender,


                    });
            }
            return rawSMSList.AsQueryable();
        }

        public IQueryable<RawSmsReceived> PageOfSellerData(MessagesIndexModel indexModel)
        {
            var rawSMSList = new List<RawSmsReceived>();

            Debug.Assert(indexModel.limit != null, "indexModel.limit != null");
            Debug.Assert(indexModel.start != null, "indexModel.start != null");
            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                rawSMSList.Add(new RawSmsReceived
                    {
                        Content = Content + "-" + i,
                        OutpostType = (OutpostType) (i%2),
                        OutpostId = Guid.NewGuid(),
                        ParseErrorMessage = "Parse error no." + i,
                        ParseSucceeded = false,
                        ReceivedDate = DateTime.UtcNow.AddDays(-i),
                        Sender = Sender,
                    });
            }
            return rawSMSList.AsQueryable();
        }
    }
}
