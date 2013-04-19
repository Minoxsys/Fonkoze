using Domain;
using System;

namespace Web.Services
{
    public interface ISendSmsService
    {
        String SendSmsRequest(SmsRequest sms);
        String SendSms(string toPhoneNumber, string message);
    }
}
