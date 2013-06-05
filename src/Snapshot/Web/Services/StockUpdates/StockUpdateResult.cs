using System.Collections.Generic;
using Web.Models.Parsing;

namespace Web.Services.StockUpdates
{
    public class StockUpdateResult
    {
        public bool Success { get; set; }
        public List<IParsedProduct> FailedProducts { get; set; }
        
    }
}