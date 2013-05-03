using System;
using System.IO;
using Web.Services.StockUpdates;

namespace Web.WarehouseMgmtUseCase.Services
{
    public interface IWarehouseManagementWorkflowService
    {
        StockUpdateResult ProcessWarehouseStockData(Stream inputData, Guid outpostId);
    }
}