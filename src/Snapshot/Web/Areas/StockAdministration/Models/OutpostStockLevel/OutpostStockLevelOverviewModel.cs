using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Persistence.Queries.Districts;
using Persistence.Queries.Countries;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OutpostStockLevelOverviewModel
    {
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Districts { get; set; }
        public List<SelectListItem> Outposts { get; set; }

        public OutpostList OutpostList { get; set; }

        public Guid OutpostId { get; set; }

        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryDistrict QueryDistrict { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        public OutpostStockLevelOverviewModel(IQueryService<Country> queryCountry)
        {
            this.QueryCountry = queryCountry;
            this.Countries = new List<SelectListItem>();
            this.Regions = new List<SelectListItem>();
            this.Districts = new List<SelectListItem>();
            this.Outposts = new List<SelectListItem>();
            this.OutpostList = new OutpostStockLevel.OutpostList();

            var countries = QueryCountry.Query();

            if (countries.ToList().Count > 0)
            {
                foreach (Country country in countries)
                {
                    this.Countries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });
                }
 
            }
        }
                           
    }
}