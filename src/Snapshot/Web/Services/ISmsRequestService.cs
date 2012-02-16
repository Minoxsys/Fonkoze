using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Services
{
    public interface ISmsRequestService
    {
        SmsRequest CreateSmsRequestUsingOutpostIdAndProductGroupId(Guid outpostId, Guid productGroupId);
        void UpdateOutpostStockLevelsWithValuesReceivedBySms(SmsReceived smsReceived);
    }
}