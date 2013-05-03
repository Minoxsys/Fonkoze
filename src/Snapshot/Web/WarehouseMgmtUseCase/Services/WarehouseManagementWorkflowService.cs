using System;
using System.IO;
using Web.Models.Parsing;
using Web.Services.StockUpdates;

namespace Web.WarehouseMgmtUseCase.Services
{
    public class WarehouseManagementWorkflowService : IWarehouseManagementWorkflowService
    {
        private readonly IStockUpdateCsvFileParserService _stockUpdateCsvFileParser;
        private readonly IUpdateStockService _updateStockService;

        public WarehouseManagementWorkflowService(IStockUpdateCsvFileParserService stockUpdateCsvFileParser, IUpdateStockService updateStockService)
        {
            _updateStockService = updateStockService;
            _stockUpdateCsvFileParser = stockUpdateCsvFileParser;
        }

        public StockUpdateResult ProcessWarehouseStockData(Stream inputData, Guid outpostId)
        {
            IParseResult parseResult = _stockUpdateCsvFileParser.ParseStream(inputData);
            if (parseResult.Success)
            {
                return _updateStockService.IncrementProductStocksForOutpost(parseResult, outpostId, StockUpdateMethod.CSV);
            }
            return new StockUpdateResult();
        }
    }
}