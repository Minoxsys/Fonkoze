using Core.Persistence;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Services;

namespace Web.Services.StockUpdates
{
    public class UpdateStockService : IUpdateStockService
    {
        private readonly ISaveOrUpdateCommand<OutpostStockLevel> _stockLevelSaveOrUpdateCommand;
        private readonly IQueryService<OutpostStockLevel> _outpostStockLevelQueryService;
        private readonly IOutpostHistoricalStockLevelService _outpostHistoricalStockLevelHistoryService;

        public UpdateStockService(ISaveOrUpdateCommand<OutpostStockLevel> stockLevelSaveOrUpdateCommand,
                                  IQueryService<OutpostStockLevel> outpostStockLevelQueryService,
                                  IOutpostHistoricalStockLevelService outpostHistoricalStockLevelHistoryService)
        {
            _outpostHistoricalStockLevelHistoryService = outpostHistoricalStockLevelHistoryService;
            _outpostStockLevelQueryService = outpostStockLevelQueryService;
            _stockLevelSaveOrUpdateCommand = stockLevelSaveOrUpdateCommand;
        }

        public void UpdateProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod)
        {
            UpdateStockTemplate(parseResult, outpostId, updateMethod, (osl, pp) => osl.StockLevel = pp.StockLevel);
        }

        public void IncrementProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod)
        {
            UpdateStockTemplate(parseResult, outpostId, updateMethod, (osl, pp) => osl.StockLevel += pp.StockLevel);
        }

        private void UpdateStockTemplate(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod,
                                         Action<OutpostStockLevel, IParsedProduct> stockUpdateStrategy)
        {
            if (!parseResult.Success)
            {
                throw new ArgumentException("Can't update stock with incorrect parse results!");
            }

            List<OutpostStockLevel> allStockLevelsForOutpost = _outpostStockLevelQueryService.Query().Where(o => o.Outpost.Id == outpostId).ToList();

            foreach (var parsedProductData in parseResult.ParsedProducts)
            {
                var stockLevel = allStockLevelsForOutpost.FirstOrDefault(
                    o =>
                    o.ProductGroup.ReferenceCode.ToLowerInvariant() == parsedProductData.ProductGroupCode.ToLowerInvariant() &&
                    o.Product.SMSReferenceCode.ToLowerInvariant() == parsedProductData.ProductCode.ToLowerInvariant());

                if (stockLevel == null) continue;

                _outpostHistoricalStockLevelHistoryService.SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(stockLevel);

                stockLevel.PrevStockLevel = stockLevel.StockLevel;

                //the place were stock changes depending on custom formula
                stockUpdateStrategy(stockLevel, parsedProductData);

                stockLevel.UpdateMethod = updateMethod.ToString();
                _stockLevelSaveOrUpdateCommand.Execute(stockLevel);
            }
        }
    }
}