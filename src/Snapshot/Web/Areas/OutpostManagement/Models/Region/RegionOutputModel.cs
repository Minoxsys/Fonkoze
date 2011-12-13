using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Country;
using System.Web.Mvc;
using Core.Persistence;
using Core.Domain;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionOutputModel
    {
        public string Name { get; set; }
        public CountryModel Country { get; set; }
        public Guid Id { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public IQueryService<Core.Domain.Country> queryCountry { get; set; }

        public RegionOutputModel() { }
        public RegionOutputModel(IQueryService<Core.Domain.Country> queryCountry)
        {

            this.queryCountry = queryCountry;

            Countries = new List<SelectListItem>();
            Country = new CountryModel();

            var result = queryCountry.Query();

            foreach (Core.Domain.Country item in result)
            {
                var selectListItem = new SelectListItem();

                selectListItem.Value = item.Id.ToString();
                selectListItem.Text = item.Name;
                Countries.Add(selectListItem);
            }
        }

    }
}