using System;
using System.IO;

namespace Web.WarehouseMgmtUseCase.Services
{
    public interface IWarehouseManagementWorkflowService
    {
        bool ProcessWarehouseStockData(Stream inputData, Guid outpostId);
    }
}