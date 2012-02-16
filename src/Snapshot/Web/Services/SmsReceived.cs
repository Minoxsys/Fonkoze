using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Domain;

namespace Web.Services
{
    public class SmsReceived
    {
        public virtual string ProductGroupReferenceCode { get; set; }
        public virtual string Number { get; set; }
        public virtual Guid RawSmsReceivedId { get; set; }
        public virtual List<ReceivedStockLevel> ReceivedStockLevels { get; set; }
    }
}
