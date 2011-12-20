using System;
using System.Collections.Generic;
using System.Linq;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using System.Web.Mvc;
using Core.Persistence;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string Email { get; set; }
        public string MainMobileNumber { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public CountryModel Country { get; set; }
        public RegionModel Region { get; set; }
        public DistrictModel District { get; set; }
        public ClientModel Client { get; set; }

        public List<SelectListItem> Outposts { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Districts { get; set; }

        public IQueryService<Domain.Country> queryCountry { get; set; }
        public IQueryService<Domain.Region> queryRegion { get; set; }
        public IQueryService<Domain.District> queryDistrict { get; set; }

                public OutpostOutputModel()
                {

                    this.queryCountry = queryCountry;
                    this.queryRegion = queryRegion;
                    this.queryDistrict = queryDistrict;

                    var Countries = new List<SelectListItem>();
                    var Regions = new List<SelectListItem>();
                    var Districts = new List<SelectListItem>();
                }

        public OutpostOutputModel(IQueryService<Domain.Country> queryCountry,
                                  IQueryService<Domain.Region> queryRegion,
                                  IQueryService<Domain.District> queryDistrict)
        {

            this.queryCountry = queryCountry;
            this.queryRegion = queryRegion;
            this.queryDistrict = queryDistrict;

            var Countries = new List<SelectListItem>();
            var Regions = new List<SelectListItem>();
            var Districts = new List<SelectListItem>();


            this.Countries = Countries;
            this.Regions = Regions;
            this.Districts = Districts;

            var Country = new CountryModel();

            var result = queryCountry.Query();

            foreach (Domain.Country item in result)
            {
                var selectListItem = new SelectListItem();

                selectListItem.Value = item.Id.ToString();
                selectListItem.Text = item.Name;
                Countries.Add(selectListItem);
            }

            var resultRegion = queryRegion.Query();

            var Region = new RegionModel();

            if (resultRegion.FirstOrDefault() != null)
            {
                foreach (Domain.Region item in resultRegion)
                {
                    var selectListItem = new SelectListItem();

                    selectListItem.Value = item.Id.ToString();
                    selectListItem.Text = item.Name;
                    Regions.Add(selectListItem);

                    var resultDistrict = queryDistrict.Query();

                    var District = new DistrictModel();

                    if (resultDistrict.FirstOrDefault() != null)
                    {
                        foreach (Domain.District item1 in resultDistrict)
                        {
                            var selectListItem1 = new SelectListItem();

                            selectListItem.Value = item1.Id.ToString();
                            selectListItem.Text = item1.Name;
                            Districts.Add(selectListItem);
                        }
                    }
                }
            }

        }

    }
}