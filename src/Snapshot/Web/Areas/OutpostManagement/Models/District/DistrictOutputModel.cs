using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;
using System.Web.Mvc;
using Core.Persistence;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RegionModel Region { get; set; }
        public ClientModel Client { get; set; }

        public List<SelectListItem> Regions { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public IQueryService<Domain.Country> QueryCountry { get; set; }

        public DistrictOutputModel() { }
        public DistrictOutputModel(IQueryService<Domain.Country> queryCountry)
        {
            this.QueryCountry = queryCountry;

            Regions = new List<SelectListItem>();
            Countries = new List<SelectListItem>();

            var countries = QueryCountry.Query();

            if (countries!=null)
            {
                foreach (Domain.Country item in countries)
                {
                    var selectListItem = new SelectListItem();

                    selectListItem.Value = item.Id.ToString();
                    selectListItem.Text = item.Name;
                    Countries.Add(selectListItem);
                }
            }
        }
    }
}