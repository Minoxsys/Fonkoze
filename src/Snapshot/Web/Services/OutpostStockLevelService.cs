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

        public OutpostHistoricalStockLevel SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel previousOutpostStockLevel)
        {
            var outpostStockLevelHistorical = new OutpostHistoricalStockLevel();
            outpostStockLevelHistorical.OutpostId = previousOutpostStockLevel.OutpostId;
            outpostStockLevelHistorical.PrevStockLevel = previousOutpostStockLevel.PrevStockLevel;
            outpostStockLevelHistorical.ProdGroupId = previousOutpostStockLevel.ProdGroupId;
            outpostStockLevelHistorical.ProdSmsRef = previousOutpostStockLevel.ProdSmsRef;
            outpostStockLevelHistorical.ProductId = previousOutpostStockLevel.ProductId;
            outpostStockLevelHistorical.StockLevel = previousOutpostStockLevel.StockLevel;
            if (previousOutpostStockLevel.Updated != null)
            {
                outpostStockLevelHistorical.UpdateDate = previousOutpostStockLevel.Updated.Value;
            }
            else
            {
                outpostStockLevelHistorical.UpdateDate = DateTime.Now;
            }
            outpostStockLevelHistorical.UpdateMethod = previousOutpostStockLevel.UpdatedMethod;
            return outpostStockLevelHistorical;
        }

        public void SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel previousOutpostStockLevel)
        {
            var outpostHistoricalStockLevel = SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(previousOutpostStockLevel);
            saveCommandOutpostHistoricalStockLevel.Execute(outpostHistoricalStockLevel);
        }
    }
}