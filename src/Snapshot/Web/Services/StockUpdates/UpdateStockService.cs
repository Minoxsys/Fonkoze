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

        public StockUpdateResult UpdateProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod)
        {
            return UpdateStockTemplate(parseResult, outpostId, updateMethod, (osl, pp) => osl.StockLevel = pp.StockLevel);
        }

        public StockUpdateResult IncrementProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod)
        {
            return UpdateStockTemplate(parseResult, outpostId, updateMethod, (osl, pp) => osl.StockLevel += pp.StockLevel);
        }

        public StockUpdateResult DecrementProductStocksForOutpost(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod)
        {
            return UpdateStockTemplate(parseResult, outpostId, updateMethod, (osl, pp) => osl.StockLevel -= pp.StockLevel);
        }

        private StockUpdateResult UpdateStockTemplate(IParseResult parseResult, Guid outpostId, StockUpdateMethod updateMethod,
                                                      Action<OutpostStockLevel, IParsedProduct> stockUpdateStrategy)
        {
            if (!parseResult.Success)
            {
                throw new ArgumentException("Can't update stock with incorrect parse results!");
            }

            List<OutpostStockLevel> allStockLevelsForOutpost = _outpostStockLevelQueryService.Query().Where(o => o.Outpost.Id == outpostId).ToList();

            var nonExistingParsedProducts = new List<IParsedProduct>();

            foreach (var parsedProductData in parseResult.ParsedProducts)
            {
                var stockLevel = allStockLevelsForOutpost.FirstOrDefault(
                    o =>
                    o.ProductGroup.ReferenceCode.ToLowerInvariant() == parsedProductData.ProductGroupCode.ToLowerInvariant() &&
                    o.Product.SMSReferenceCode.ToLowerInvariant() == parsedProductData.ProductCode.ToLowerInvariant());

                if (stockLevel == null)
                {
                    nonExistingParsedProducts.Add(parsedProductData);
                    continue;
                }

                _outpostHistoricalStockLevelHistoryService.SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(stockLevel);

                stockLevel.PrevStockLevel = stockLevel.StockLevel;

                //the place were stock changes depending on custom formula
                stockUpdateStrategy(stockLevel, parsedProductData);

                stockLevel.UpdateMethod = updateMethod.ToString();
                _stockLevelSaveOrUpdateCommand.Execute(stockLevel);
            }

            return new StockUpdateResult
                {
                    Success = !nonExistingParsedProducts.Any(),
                    FailedProducts = nonExistingParsedProducts.Any() ? nonExistingParsedProducts : null
                };
        }
    }
}