using Domain;
using System;

namespace Web.Services
{
    public interface ISmsGatewayService
    {
        String SendSmsRequest(SmsRequest sms);
        String SendSms(string toPhoneNumber, string message);
    }
}
