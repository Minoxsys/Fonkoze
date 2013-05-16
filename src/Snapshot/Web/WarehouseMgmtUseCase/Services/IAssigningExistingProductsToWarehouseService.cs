using System;
using System.Collections.Generic;
using Web.Models.Parsing;
using Web.Services.StockUpdates;

namespace Web.WarehouseMgmtUseCase.Services
{
    public interface IAssigningExistingProductsToWarehouseService
    {
        void AssigningProductsToWarehouse(IEnumerable<IParsedProduct> inputData, Guid outpostId);
    }
}
