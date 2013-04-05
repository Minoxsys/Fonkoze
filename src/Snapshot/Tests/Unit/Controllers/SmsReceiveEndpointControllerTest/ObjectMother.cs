using Core.Domain;
using Core.Persistence;
using Domain;
using MvcContrib.TestHelper.Fakes;
using Rhino.Mocks;
using System;
using Web.Controllers;
using Web.Services;

namespace Tests.Unit.Controllers.SmsReceiveEndpointControllerTest
{
    public class ObjectMother
    {
        public const string MOBILE_NUMBER = "0123456789";
        public const string SMS_CONTENT = "Malaria R0#1";
        public const string IN_NUMBER = "1234";
        public const string EMAIL = "a@a.ro";
        public const string CREDITS = "10";
        public const string USERNAME = "admin";

        public Guid clientId;
        public Guid userId;
        public Guid outpostId;
        public Guid productGroupId;
        public Guid smsRequestId;

        public Client client;
        public User user;
        public Outpost outpost;
        public ProductGroup productGroup;
        public SmsRequest smsRequest;
        public SmsRequest nullSmsRequest;
        public RawSmsReceived rawSmsReceived;
        public RawSmsReceivedParseResult rawSmsReceivedParseResult;
        public RawSmsReceivedParseResult rawSmsReceivedParseResultFailed;
        public SmsReceived smsReceived;

        public IQueryService<User> queryServiceUsers { get; set; }
        public IQueryService<Client> queryServiceClients { get; set; }

        public IQueryService<Outpost> queryServiceOutpost;

        public ISaveOrUpdateCommand<RawSmsReceived> saveCommandRawSmsReceived;
        public ISaveOrUpdateCommand<Alert> saveAlertCmd;

        public ISmsRequestService smsRequestService;
        public ISmsGatewayService smsGatewayService;

        public SmsReceiveEndpointController controller;

        public void Setup_Controller_And_Mock_Services()
        {
            controller = new SmsReceiveEndpointController();
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();

            smsRequestService = MockRepository.GenerateMock<ISmsRequestService>();
            smsGatewayService = MockRepository.GenerateMock<ISmsGatewayService>();

            saveCommandRawSmsReceived = MockRepository.GenerateMock<ISaveOrUpdateCommand<RawSmsReceived>>();
            saveAlertCmd = MockRepository.GenerateMock<ISaveOrUpdateCommand<Alert>>();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryOutpost = queryServiceOutpost;
            controller.SmsRequestService = smsRequestService;
            controller.SmsGatewayService = smsGatewayService;
            controller.SaveCommandRawSmsReceived = saveCommandRawSmsReceived;
            controller.SaveAlertCmd = saveAlertCmd;
        }

        public void SetUp_StubData()
        {
            clientId = Guid.NewGuid();

            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(u => u.Id).Return(userId);
            user.UserName = USERNAME;
            user.ClientId = clientId;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = "Spitalul Judetean";
            outpost.Country = new Country();
            outpost.Region = new Region();
            outpost.District = new District();

            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = "AIDS";

            smsRequestId = Guid.NewGuid();
            smsRequest = MockRepository.GeneratePartialMock<SmsRequest>();
            smsRequest.Stub(c => c.Id).Return(smsRequestId);
            smsRequest.Message = SMS_CONTENT;
            smsRequest.Number = MOBILE_NUMBER;
            smsRequest.OutpostId = outpostId;
            smsRequest.ProductGroupId = productGroupId;

            nullSmsRequest = MockRepository.GeneratePartialMock<SmsRequest>();

            rawSmsReceived = new RawSmsReceived()
            {
                Content = SMS_CONTENT, Credits = CREDITS, Sender = MOBILE_NUMBER, OutpostId = outpostId
            };

            rawSmsReceivedParseResult = MockRepository.GeneratePartialMock<RawSmsReceivedParseResult>();
            rawSmsReceivedParseResult.Stub(r => r.SmsReceived).Return(smsReceived);
            rawSmsReceivedParseResult.ParseSucceeded = true;

            rawSmsReceivedParseResultFailed = MockRepository.GeneratePartialMock<RawSmsReceivedParseResult>();
            rawSmsReceivedParseResultFailed.Stub(r => r.SmsReceived).Return(null);
            rawSmsReceivedParseResultFailed.ParseSucceeded = false;
        }
    }
}
