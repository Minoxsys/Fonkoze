using System;
using System.IO;
using Web.Models.Parsing;
using Web.Services.StockUpdates;
using Web.WarehouseMgmtUseCase.Model;

namespace Web.WarehouseMgmtUseCase.Services
{
    public class WarehouseManagementWorkflowService : IWarehouseManagementWorkflowService
    {
        private readonly IStockUpdateCsvFileParserService _stockUpdateCsvFileParser;
        private readonly IUpdateStockService _updateStockService;
        private readonly IAssigningExistingProductsToWarehouseService _assigningExistingProductsToWarehouseService;

        public WarehouseManagementWorkflowService(IStockUpdateCsvFileParserService stockUpdateCsvFileParser, IUpdateStockService updateStockService, IAssigningExistingProductsToWarehouseService assigningExistingProductsToWarehouseService)
        {
            _updateStockService = updateStockService;
            _stockUpdateCsvFileParser = stockUpdateCsvFileParser;
            _assigningExistingProductsToWarehouseService = assigningExistingProductsToWarehouseService;
        }

        public StockUpdateResult ProcessWarehouseStockData(Stream inputData, Guid outpostId)
        {
            IParseResult parseResult = _stockUpdateCsvFileParser.ParseStream(inputData);
            if (parseResult.Success)
            {
                var stockResult = _updateStockService.IncrementProductStocksForOutpost(parseResult, outpostId, StockUpdateMethod.CSV);

                if (stockResult.FailedProducts == null)
                    return stockResult;

                _assigningExistingProductsToWarehouseService.AssigningProductsToWarehouse(stockResult.FailedProducts, outpostId);

                return _updateStockService.IncrementProductStocksForOutpost(
                    new CsvParseResult() { Success = true, ParsedProducts = stockResult.FailedProducts }, outpostId, StockUpdateMethod.CSV);
            }
            return new StockUpdateResult();
        }
    }
}