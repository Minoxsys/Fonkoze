using Domain;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface IOutpostHistoricalStockLevelService
    {
        void SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel previousOutpostStockLevel);
    }
}