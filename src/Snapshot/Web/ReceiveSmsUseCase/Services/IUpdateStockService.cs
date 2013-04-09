
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface IUpdateStockService
    {
        void UpdateProductStocks(ISmsParseResult parseResult);
    }
}
