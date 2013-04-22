using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Services;
using Rhino.Mocks;
using Domain;
using System.Web;
using Web.Bootstrap;
using Core.Persistence;
using Persistence.Queries.Outposts;

namespace Tests.Unit.Services.SmsGatewayServiceTest
{
    public class ObjectMother
    {
        public const string SMS_GATEWAY_URL = "https://www.txtlocal.com/sendsmspost.php";
        public const string SMS_GATEWAY_USER_NAME = "geo_nasa@yahoo.com";
        public const string SMS_GATEWAY_PASSWORD = "asd123";
        public const string SMS_GATEWAY_FROM = "xreplyx";
        public const string SMS_GATEWAY_TESTMODE = "1";
        public const string SMS_GATEWAY_DEBUG = "0";

        public const string SMS_MESSAGE_TEMPLATE = "Please provide current stock level for product group {0} using format\n{1} {2}";
        public const string PRODUCT_GROUP_NAME= "Malaria";
        public const string PRODUCT_GROUP_REFERENCE_CODE = "MAL";
        public const string OUTPOST_STOCK_LEVELS = "R0J0";
        public const string BAD_MESSAGE = "Looks good";
        public const string RECEIVED_MESSAGE = "Mal R1J2";
        public const string NUMBER = "1234567890";
        public const string WRONG_NUMBER = "4321";

        public Guid smsId;
        public Guid rawSmsReceivedId;
        public Guid outpostId;
        public string postRequest;

        public ISmsGatewaySettingsService fakeSmsGatewaySettingsService;
        public IHttpService fakeHttpService;
        public ISendSmsService smsGatewayService;

        public IQueryOutposts queryOutposts;

        public IQueryService<Contact> queryServiceContact;
        public ISaveOrUpdateCommand<SentSms> sentSmsSaveOrUpdateCmd; 

        public SmsRequest smsRequest;
        public RawSmsReceived rawSmsReceived;
        public RawSmsReceived rawSmsReceivedWithBadContent;
        public RawSmsReceived rawSmsReceivedWithWrongNumber;
        public Outpost outpost;
        public Contact contact;

        public void SetupSmsService_and_MockServices()
        {
            fakeHttpService = MockRepository.GenerateMock<IHttpService>();

            fakeSmsGatewaySettingsService = MockRepository.GeneratePartialMock<SmsGatewaySettingsService>();
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayUrl).Return(SMS_GATEWAY_URL);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayUserName).Return(SMS_GATEWAY_USER_NAME);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayPassword).Return(SMS_GATEWAY_PASSWORD);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayFrom).Return(SMS_GATEWAY_FROM);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayTestMode).Return(SMS_GATEWAY_TESTMODE);
            fakeSmsGatewaySettingsService.Stub(c => c.SmsGatewayDebugMode).Return(SMS_GATEWAY_DEBUG);

            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();
            sentSmsSaveOrUpdateCmd = MockRepository.GenerateMock<ISaveOrUpdateCommand<SentSms>>();
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();
            smsGatewayService = new SendSmsService(fakeSmsGatewaySettingsService, fakeHttpService, sentSmsSaveOrUpdateCmd);
        }

        public void SetUp_StubData()
        {
            smsId = Guid.NewGuid();
            smsRequest = MockRepository.GeneratePartialMock<SmsRequest>();
            smsRequest.Stub(c => c.Id).Return(smsId);
            smsRequest.Message = string.Format(SMS_MESSAGE_TEMPLATE, PRODUCT_GROUP_NAME, PRODUCT_GROUP_REFERENCE_CODE, OUTPOST_STOCK_LEVELS);
            smsRequest.Number = NUMBER;

            rawSmsReceivedId = Guid.NewGuid();
            rawSmsReceived = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsReceived.Stub(c => c.Id).Return(rawSmsReceivedId);
            rawSmsReceived.Sender = NUMBER;
            rawSmsReceived.Content = RECEIVED_MESSAGE;

            rawSmsReceivedWithWrongNumber = new RawSmsReceived() { Sender = WRONG_NUMBER };
            rawSmsReceivedWithBadContent = new RawSmsReceived() { Content = "" };

            contact = new Contact() { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = NUMBER, IsMainContact = true };

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Contacts = new Contact[] { contact };

            postRequest = "uname=geo_nasa@yahoo.com&pword=asd123&from=xreplyx&test=1&info=0&selectednums=1234567890&custom=" + smsId + "&message=" + HttpUtility.HtmlEncode(string.Format(SMS_MESSAGE_TEMPLATE, PRODUCT_GROUP_NAME, PRODUCT_GROUP_REFERENCE_CODE, OUTPOST_STOCK_LEVELS));
        }
    }
}
