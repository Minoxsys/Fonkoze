using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Core.Persistence;

namespace Web.Services
{
    public class OutpostStockLevelService : IOutpostStockLevelService
    {
        private ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveCommandOutpostHistoricalStockLevel;

        public OutpostStockLevelService(ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveCommandOutpostHistoricalStockLevel)
        {
            this.saveCommandOutpostHistoricalStockLevel = saveCommandOutpostHistoricalStockLevel;
        }

        public OutpostHistoricalStockLevel SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel outpostStockLevel)
        {
            var outpostHistoricalStockLevel = new OutpostHistoricalStockLevel();

            outpostHistoricalStockLevel.OutpostId = outpostStockLevel.Outpost.Id;
            outpostHistoricalStockLevel.OutpostName = outpostStockLevel.Outpost.Name;

            outpostHistoricalStockLevel.PrevStockLevel = outpostStockLevel.PrevStockLevel;

            outpostHistoricalStockLevel.ProductGroupId = outpostStockLevel.ProductGroup.Id;
            outpostHistoricalStockLevel.ProductGroupName = outpostStockLevel.ProductGroup.Name;

            outpostHistoricalStockLevel.ProdSmsRef = outpostStockLevel.Product.SMSReferenceCode;
            outpostHistoricalStockLevel.ProductId = outpostStockLevel.Product.Id;
            outpostHistoricalStockLevel.ProductName = outpostStockLevel.Product.Name;

            outpostHistoricalStockLevel.StockLevel = outpostStockLevel.StockLevel;
            outpostHistoricalStockLevel.UpdateDate = outpostStockLevel.Updated;
            outpostHistoricalStockLevel.UpdateMethod = outpostStockLevel.UpdateMethod;

            return outpostHistoricalStockLevel;
        }

        public void SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel previousOutpostStockLevel)
        {
            var outpostHistoricalStockLevel = SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(previousOutpostStockLevel);
            saveCommandOutpostHistoricalStockLevel.Execute(outpostHistoricalStockLevel);
        }
    }
}