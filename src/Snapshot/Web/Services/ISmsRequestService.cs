using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Services
{
    public interface ISmsRequestService
    {
        SmsRequest CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(Guid outpostId, Guid productGroupId, Client client);
        List<SmsRequest> CreateSmsRequestsForProductLevelRequestForClient(ProductLevelRequest productLevelRequest, Client client);
        void UpdateOutpostStockLevelsWithValuesReceivedBySms(SmsReceived smsReceived);
    }
}