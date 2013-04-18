using System;
using System.IO;

namespace Web.WarehouseMgmtUseCase.Services
{
    public interface IWarehouseManagementWorkflowService
    {
        void ProcessWarehouseStockData(Stream inputData, Guid outpostId);
    }
}