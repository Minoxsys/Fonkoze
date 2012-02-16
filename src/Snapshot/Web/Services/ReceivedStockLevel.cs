using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Web.Services
{
    public class ReceivedStockLevel
    {
        public virtual string ProductSmsReference { get; set; }
        public virtual int StockLevel { get; set; }
    }
}
