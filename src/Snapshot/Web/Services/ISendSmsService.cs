using Domain;

namespace Web.Services
{
    public interface ISendSmsService
    {
        string SendSmsRequest(SmsRequest sms, bool saveRequest);
        string SendSms(string toPhoneNumber, string message, bool saveMessage);
    }
}
