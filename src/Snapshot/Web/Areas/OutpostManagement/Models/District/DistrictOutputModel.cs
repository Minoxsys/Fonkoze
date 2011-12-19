using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Region;
using System.Web.Mvc;
using Core.Persistence;
using Web.Areas.OutpostManagement.Models.Client;

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

        public IQueryService<Domain.Region> QueryRegion { get; set; }

        public DistrictOutputModel() { }
        public DistrictOutputModel(IQueryService<Domain.Country> queryCountry, IQueryService<Domain.Region> queryRegion)
        {
            this.QueryCountry = queryCountry;
            this.QueryRegion = queryRegion;

            Regions = new List<SelectListItem>();
            Countries = new List<SelectListItem>();

            var countries = QueryCountry.Query().OrderBy(m=>m.Name).ToList();


            foreach (Domain.Country item in countries)
            {
                var selectListItem = new SelectListItem();

                selectListItem.Value = item.Id.ToString();
                selectListItem.Text = item.Name;
                Countries.Add(selectListItem);
            }

            if (countries.Count > 0)
            {
                var country = countries.First();
                var regions = QueryRegion.Query().Where<Domain.Region>(it=>it.Country.Id == country.Id).OrderBy(m => m.Name).ToList();

                foreach (Domain.Region item in regions)
                {
                    var selectListItem = new SelectListItem();

                    selectListItem.Value = item.Id.ToString();
                    selectListItem.Text = item.Name;
                    Regions.Add(selectListItem);
                }
            }

        }
    }
}