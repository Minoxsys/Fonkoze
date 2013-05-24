using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using Domain;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;

namespace Web.Services
{
    public class RetrieveAllDistrictsService : IRetrieveAllDistrictsService
    {
        private readonly IQueryService<District> _queryDistrictService;

        public RetrieveAllDistrictsService(IQueryService<District> queryDistrictService)
        {
            _queryDistrictService = queryDistrictService;
        }

        public List<EntityModel> GetAllDistrictsForOneClient(Guid clientId)
        {
            var districts = new List<District>();
            var districtList = new List<EntityModel>();
            var allModel = new EntityModel { Id = Guid.Empty, Name = " All" };
            districtList.Add(allModel);


            districts = _queryDistrictService.Query().Where(it => it.Client.Id == clientId).ToList();

            foreach (var district in districts)
            {
                var model = new EntityModel();
                model.Name = district.Name;
                model.Id = district.Id;
                districtList.Add(model);
            }

            return districtList;
        }
    }
}