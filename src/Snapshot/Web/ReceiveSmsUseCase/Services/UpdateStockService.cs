using Core.Persistence;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public class UpdateStockService : IUpdateStockService
    {
        private const string UpdateMethodSms = "SMS";
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

        public void UpdateProductStocksForOutpost(ISmsParseResult parseResult, Guid outpostId)
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

                if (stockLevel != null)
                {
                    _outpostHistoricalStockLevelHistoryService.SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(stockLevel);

                    stockLevel.PrevStockLevel = stockLevel.StockLevel;
                    stockLevel.StockLevel = parsedProductData.StockLevel;
                    stockLevel.UpdateMethod = UpdateMethodSms;
                    _stockLevelSaveOrUpdateCommand.Execute(stockLevel);
                }
            }
        }
    }
}