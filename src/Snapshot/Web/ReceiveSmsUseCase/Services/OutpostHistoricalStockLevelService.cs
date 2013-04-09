using Core.Persistence;
using Domain;

namespace Web.ReceiveSmsUseCase.Services
{
    public class OutpostHistoricalStockLevelService : IOutpostHistoricalStockLevelService
    {
        private readonly ISaveOrUpdateCommand<OutpostHistoricalStockLevel> _saveCommandOutpostHistoricalStockLevel;

        public OutpostHistoricalStockLevelService(ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveCommandOutpostHistoricalStockLevel)
        {
            _saveCommandOutpostHistoricalStockLevel = saveCommandOutpostHistoricalStockLevel;
        }

        private OutpostHistoricalStockLevel SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel outpostStockLevel)
        {
            var outpostHistoricalStockLevel = new OutpostHistoricalStockLevel
                {
                    OutpostId = outpostStockLevel.Outpost.Id,
                    OutpostName = outpostStockLevel.Outpost.Name,
                    PrevStockLevel = outpostStockLevel.PrevStockLevel,
                    ProductGroupId = outpostStockLevel.ProductGroup.Id,
                    ProductGroupName = outpostStockLevel.ProductGroup.Name,
                    ProdSmsRef = outpostStockLevel.Product.SMSReferenceCode,
                    ProductId = outpostStockLevel.Product.Id,
                    ProductName = outpostStockLevel.Product.Name,
                    StockLevel = outpostStockLevel.StockLevel,
                    UpdateDate = outpostStockLevel.Updated,
                    UpdateMethod = outpostStockLevel.UpdateMethod
                };

            return outpostHistoricalStockLevel;
        }

        public void SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel previousOutpostStockLevel)
        {
            var outpostHistoricalStockLevel = SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(previousOutpostStockLevel);
            _saveCommandOutpostHistoricalStockLevel.Execute(outpostHistoricalStockLevel);
        }
    }
}