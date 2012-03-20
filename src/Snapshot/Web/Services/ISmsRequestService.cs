using System;
using System.Linq;

namespace Web.Services
{
    public interface ISmsRequestService
    {
        void UpdateOutpostStockLevelsWithValuesReceivedBySms(SmsReceived smsReceived);
    }
}