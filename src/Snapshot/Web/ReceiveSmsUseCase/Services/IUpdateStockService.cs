
using System;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface IUpdateStockService
    {
        void UpdateProductStocksForOutpost(ISmsParseResult parseResult, Guid outpostId);
    }
}
