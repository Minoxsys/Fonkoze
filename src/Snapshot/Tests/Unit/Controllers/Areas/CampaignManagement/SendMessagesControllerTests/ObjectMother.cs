using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistence.Queries.Outposts;
using Web.Services;
using Rhino.Mocks;
using Core.Persistence;
using Domain;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.SendMessagesControllerTests
{
    public class ObjectMother
    {
        public const string SMS_GATEWAY_URL = "https://www.txtlocal.com/sendsmspost.php";
        public const string SMS_GATEWAY_USER_NAME = "geo_nasa@yahoo.com";
        public const string SMS_GATEWAY_PASSWORD = "asd123";
        public const string SMS_GATEWAY_FROM = "xreplyx";
        public const string SMS_GATEWAY_TESTMODE = "1";
        public const string SMS_GATEWAY_DEBUG = "0";

        public ISmsGatewaySettingsService fakeSmsGatewaySettingsService;
        public ISendSmsService smsGatewayService;
        public IQueryOutposts queryOutposts;
        public IHttpService fakeHttpService;
        public ISaveOrUpdateCommand<SentSms> SaveOrUpdateCommand;

        public void SetupSmsService_and_MockServices()
        {
            fakeHttpService = MockRepository.GenerateMock<IHttpService>();

            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();
            fakeSmsGatewaySettingsService = MockRepository.GeneratePartialMock<SmsGatewaySettingsService>();
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayUrl).Return(SMS_GATEWAY_URL);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayUserName).Return(SMS_GATEWAY_USER_NAME);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayPassword).Return(SMS_GATEWAY_PASSWORD);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayFrom).Return(SMS_GATEWAY_FROM);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayTestMode).Return(SMS_GATEWAY_TESTMODE);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayDebugMode).Return(SMS_GATEWAY_DEBUG);
            smsGatewayService = new SendSmsService(fakeSmsGatewaySettingsService, fakeHttpService);

            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();

        }

    }
}
