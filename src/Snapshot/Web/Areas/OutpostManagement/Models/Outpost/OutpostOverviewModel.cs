using System;
using System.Collections.Generic;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Contact;
using System.Linq;
using System.Web.Mvc;
using Core.Persistence;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostOverviewModel
    {
       public string Name { get; set; }
       public string OutpostType { get; set; }
       public string DetailMethod { get; set; }
       public string Longitude { get; set; }
       public string Latitude { get; set; }
       public CountryModel Country { get; set; }
       public RegionModel Region { get; set; }
       public DistrictModel District { get; set; }
        
       public List<SelectListItem> Countries { get; set; }
       public List<SelectListItem> Regions { get; set; }
       public List<SelectListItem> Districts { get; set; }

       public List<OutpostModel> Outposts { get; set; }
       public List<ContactModel> Contact { get; set; }
       public List<Domain.Contact> Contacts { get; set; }

       public IQueryService<Domain.Country> QueryCountry { get; set; }
       public IQueryService<Domain.Region> QueryRegion { get; set; }
       public IQueryService<Domain.District> QueryDistrict { get; set; }
  
        public string Error { get; set; }

        public OutpostOverviewModel()
        {
            this.Districts = new List<SelectListItem>();
            this.Countries = new List<SelectListItem>();
            this.Regions = new List<SelectListItem>();
            //----------------------------------------
            this.Outposts = new List<OutpostModel>();
       }

         public OutpostOverviewModel(IQueryService<Domain.Country> queryCountry, 
                                     IQueryService<Domain.Region> queryRegion,
                                     IQueryService<Domain.District> queryDistrict)
        {
            this.QueryCountry = queryCountry;
            this.QueryRegion = queryRegion;
            this.QueryDistrict = queryDistrict;
            this.Countries = new List<SelectListItem>();
            this.Regions = new List<SelectListItem>();
            this.Districts = new List<SelectListItem>();

            this.Outposts = new List<OutpostModel>();
            this.Contact = new List<ContactModel>();

            var countries = QueryCountry.Query().OrderBy(it => it.Name);

            foreach (Domain.Country country in countries)
            {
                this.Countries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });
            }

            Guid firstCountrySelected = new Guid();

            if (countries.ToList().Count > 0)
            {
                firstCountrySelected = Guid.Parse(this.Countries.First().Value);
            }

            var regions = QueryRegion.Query().Where(it => it.Country.Id == firstCountrySelected);

            foreach (var region in regions)
            {
                this.Regions.Add(new SelectListItem { Text = region.Name, Value = region.Id.ToString() });

                var regionID = region.Id;

                var districts = QueryDistrict.Query().Where(it => it.Region.Id == regionID);

                foreach (var district in districts)
                {
                    this.Districts.Add(new SelectListItem { Text = district.Name, Value = district.Id.ToString() });
                }
            }
             
             
        }


    }
}