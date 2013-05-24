using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;

namespace Web.Services
{
    public interface IRetrieveAllDistrictsService
    {
        List<EntityModel> GetAllDistrictsForOneClient(Guid clientId);
    }
}
