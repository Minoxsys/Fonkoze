using Domain;
using System.Text;
using System.Web;

namespace Web.Services
{
    public class SmsGatewayService : ISmsGatewayService
    {
        private readonly IHttpService _httpService;
        private readonly ISmsGatewaySettingsService _smsGatewaySettingsService;

        public SmsGatewayService(ISmsGatewaySettingsService smsGatewaySettingsService, IHttpService httpService)
        {
            _httpService = httpService;
            _smsGatewaySettingsService = smsGatewaySettingsService;
        }

        public string SendSms(string toPhoneNumber, string message)//TODO: customize to the ways of the new gateway
        {
           string postData = GetPostDataFromSettings() + "&selectednums"+ toPhoneNumber +"&message=" + HttpUtility.HtmlEncode(message); 
           string postResponse = _httpService.Post(_smsGatewaySettingsService.SmsGatewayUrl, postData);
           return postResponse;
        }

        public string SendSmsRequest(SmsRequest smsRequest)
        {
            string postData = GetPostDataFromSettings() + "&" + GetPostDataFromSmsRequest(smsRequest); 
            string postResponse = _httpService.Post(_smsGatewaySettingsService.SmsGatewayUrl, postData);
            return postResponse;
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