using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.District;
using System.Web.Mvc;
using Core.Persistence;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionOverviewModel
    {
        public List<RegionModel> Regions { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public Guid countryId { get; set; }

        public IQueryService<Domain.Country> QueryCountry { get; set; }

        public RegionOverviewModel()
        {
            Regions = new List<RegionModel>();
            Countries = new List<SelectListItem>();
        }
        public RegionOverviewModel(IQueryService<Domain.Country> queryCountry)
        {

            Regions = new List<RegionModel>();
            Countries = new List<SelectListItem>();

            this.QueryCountry = queryCountry;
            
            var countries = QueryCountry.Query().ToList();

            foreach (Domain.Country item in countries)
            {
                this.Countries.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

        }
        public string Error { get; set; }
    }
}