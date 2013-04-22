using System;
using Core.Persistence;
using Domain;
using System.Text;
using System.Web;

namespace Web.Services
{
    public class SendSmsService : ISendSmsService
    {
        private readonly IHttpService _httpService;
        private readonly ISmsGatewaySettingsService _smsGatewaySettingsService;
        private readonly ISaveOrUpdateCommand<SentSms> _saveSentSmsCommand;

        public SendSmsService(ISmsGatewaySettingsService smsGatewaySettingsService, IHttpService httpService, ISaveOrUpdateCommand<SentSms> saveSentSmsCommand)
        {
            _saveSentSmsCommand = saveSentSmsCommand;
            _httpService = httpService;
            _smsGatewaySettingsService = smsGatewaySettingsService;
        }

        public string SendSms(string toPhoneNumber, string message, bool saveMessage) //TODO: customize to the ways of the new gateway
        {
            string postData = GetPostDataFromSettings() + "&selectednums=" + toPhoneNumber + "&message=" + HttpUtility.HtmlEncode(message);
            string postResponse = _httpService.Post(_smsGatewaySettingsService.SmsGatewayUrl, postData);
            SaveMessage(toPhoneNumber, message, postResponse);
            return postResponse;
        }

        public string SendSmsRequest(SmsRequest smsRequest, bool saveRequest)
        {
            string postData = GetPostDataFromSettings() + "&" + GetPostDataFromSmsRequest(smsRequest);
            string postResponse = _httpService.Post(_smsGatewaySettingsService.SmsGatewayUrl, postData);
            SaveMessage(smsRequest.Number, smsRequest.Message, postResponse);
            return postResponse;
        }

        private void SaveMessage(string sentTo, string message, string responseString)
        {
            var sentSms = new SentSms {PhoneNumber = "+" + sentTo, Message = message, Response = responseString, SentDate = DateTime.UtcNow};
            _saveSentSmsCommand.Execute(sentSms);
        }

        private string GetPostDataFromSettings()
        {
            var postDataBuilder = new StringBuilder();
            postDataBuilder.Append("uname=" + _smsGatewaySettingsService.SmsGatewayUserName);
            postDataBuilder.Append("&pword=" + _smsGatewaySettingsService.SmsGatewayPassword);
            postDataBuilder.Append("&from=" + _smsGatewaySettingsService.SmsGatewayFrom);
            postDataBuilder.Append("&test=" + _smsGatewaySettingsService.SmsGatewayTestMode);
            postDataBuilder.Append("&info=" + _smsGatewaySettingsService.SmsGatewayDebugMode);

            return postDataBuilder.ToString();
        }

        private string GetPostDataFromSmsRequest(SmsRequest smsRequest)
        {
            var postDataBuilder = new StringBuilder();
            postDataBuilder.Append("selectednums=" + smsRequest.Number);
            postDataBuilder.Append("&custom=" + smsRequest.Id);
            postDataBuilder.Append("&message=" + HttpUtility.HtmlEncode(smsRequest.Message));

            return postDataBuilder.ToString();
        }
    }
}