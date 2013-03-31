using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictOverviewModel
    {
        public List<DistrictModel> Districts { get; set; }

        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public string Error { get; set; }

        public IQueryService<Domain.Country> QueryCountry { get; set; }
       
        public DistrictOverviewModel()
        {
            this.Districts = new List<DistrictModel>();
            this.Countries = new List<SelectListItem>();
            this.Regions = new List<SelectListItem>();
        }

        public DistrictOverviewModel(IQueryService<Domain.Country> queryCountry)
        {
            this.QueryCountry = queryCountry;
            this.Countries = new List<SelectListItem>();
            this.Regions = new List<SelectListItem>();
            this.Districts = new List<DistrictModel>();

            var countries = QueryCountry.Query().OrderBy(it => it.Name);

            foreach (Domain.Country country in countries)
            {
                this.Countries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });
            }
       }
    }
}