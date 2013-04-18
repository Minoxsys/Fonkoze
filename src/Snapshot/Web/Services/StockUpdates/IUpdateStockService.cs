using System;
using Web.Models.Parsing;

namespace Web.Services.StockUpdates
{
    public interface IUpdateStockService
    {
        void UpdateProductStocksForOutpost(IParseResult parseResult, Guid outpostId);
    }
}
