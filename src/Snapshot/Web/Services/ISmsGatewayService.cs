using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Web.Services
{
    public interface ISmsGatewayService
    {
        String SendSmsRequest(SmsRequest sms);

        RawSmsReceived AssignOutpostToRawSmsReceivedBySenderNumber(RawSmsReceived rawSmsReceived);

        RawSmsReceivedParseResult ParseRawSmsReceived(RawSmsReceived rawSmsReceived);
    }
}
