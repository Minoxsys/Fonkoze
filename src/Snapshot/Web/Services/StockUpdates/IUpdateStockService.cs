using System;
using Web.Models.Parsing;

namespace Web.Services.StockUpdates
{
    public interface IUpdateStockService
    {
        StockUpdateResult UpdateProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod);
        StockUpdateResult IncrementProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod);
        StockUpdateResult DecrementProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod);
    }
}
