using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Core.Persistence;
using Domain;
using Web.Services.StockUpdates;
using Web.Models.Parsing;

namespace Web.WarehouseMgmtUseCase.Services
{
    public class AssigningExistingProductsToWarehouseService : IAssigningExistingProductsToWarehouseService
    {
        private readonly IQueryService<Product> _productQueryService;
        private readonly IQueryService<Outpost> _outpostQueryService;

        private readonly ISaveOrUpdateCommand<OutpostStockLevel> _saveOutpostStockLevelCommand;

        public AssigningExistingProductsToWarehouseService(IQueryService<Product> productQueryService, IQueryService<Outpost> outpostQueryService, ISaveOrUpdateCommand<OutpostStockLevel> saveOutpostStockLevelCommand)
        {
            _productQueryService = productQueryService;
            _outpostQueryService = outpostQueryService;

            _saveOutpostStockLevelCommand = saveOutpostStockLevelCommand;
        }

        public void AssigningProductsToWarehouse(IEnumerable<IParsedProduct> inputData, Guid outpostId)
        {
            foreach (var p in inputData)
            {
                var product = _productQueryService.Query().
                    Where(o => o.SMSReferenceCode.Equals(p.ProductCode)
                        && o.ProductGroup.ReferenceCode.Equals(p.ProductGroupCode)).FirstOrDefault();

                if (product != null)
                {
                    var outpostStockItem = new OutpostStockLevel
                    {
                        Product = product,
                        Outpost = _outpostQueryService.Load(outpostId),
                        ProductGroup = product.ProductGroup,
                        UpdateMethod = OutpostStockLevel.MANUAL_UPDATE,
                        Client = product.Client,
                        ByUser = product.ByUser
                    };

                    _saveOutpostStockLevelCommand.Execute(outpostStockItem);
                }
            }
        }
    }
}