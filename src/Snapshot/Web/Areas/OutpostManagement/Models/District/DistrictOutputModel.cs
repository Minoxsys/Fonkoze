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
        public DistrictOutputModel(IQueryService<Domain.Country> queryCountry, IQueryService<Domain.Region> queryRegion,Guid? countryId,Guid? regionId)
        {
            this.QueryCountry = queryCountry;
            this.QueryRegion = queryRegion;

            Region = new RegionModel();
            Client = new ClientModel();
            Regions = new List<SelectListItem>();
            Countries = new List<SelectListItem>();

            var countries = QueryCountry.Query().OrderBy(m => m.Name).ToList();


            foreach (Domain.Country item in countries)
            {
                var selectListItem = new SelectListItem();

                selectListItem.Value = item.Id.ToString();
                selectListItem.Text = item.Name;
                Countries.Add(selectListItem);
            }

            if (countryId != null)
            {
                if (this.Countries.Where(it => it.Value == countryId.Value.ToString()).ToList().Count > 0)
                    this.Countries.First(it => it.Value == countryId.Value.ToString()).Selected = true;

                var regions = QueryRegion.Query().Where(it => it.Country.Id == countryId.Value).ToList();

                foreach (Domain.Region region in regions)
                {
                    if (regionId != null)
                    {
                        this.Regions.Add(new SelectListItem { Text = region.Name, Value = region.Id.ToString(), Selected = region.Id == regionId });
                    }
                    else
                    {
                        this.Regions.Add(new SelectListItem { Text = region.Name, Value = region.Id.ToString()});
                    }
                }

            }
            
        }
        public DistrictOutputModel(IQueryService<Domain.Country> queryCountry)
        {
            this.QueryCountry = queryCountry;

            Region = new RegionModel();
            Client = new ClientModel();
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
           
        }
    }
}