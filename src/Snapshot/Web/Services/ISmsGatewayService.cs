using Domain;
using System;

namespace Web.Services
{
    public interface ISmsGatewayService
    {
        String SendSmsRequest(SmsRequest sms);
    }
}
