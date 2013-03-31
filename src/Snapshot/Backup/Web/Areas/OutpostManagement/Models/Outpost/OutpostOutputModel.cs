using System;
using System.Collections.Generic;
using System.Linq;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Contact;
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
        public string DetailMethod { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool IsWarehouse { get; set; }
        public RegionModel Region { get; set; }
        public DistrictModel District { get; set; }
        public ClientModel Client { get; set; }
        public ContactModel Contact { get; set; }
        public OutpostModel Warehouse { get; set; }
        public List<Domain.Contact> Contacts { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Districts { get; set; }
        public List<SelectListItem> Warehouses { get; set; }

        public List<SelectListItem> Outposts { get; set; }

        public IQueryService<Domain.Outpost> queryWarehouse { get; set; }
        public IQueryService<Domain.Country> queryCountry { get; set; }
        public IQueryService<Domain.Region> queryRegion { get; set; }
        public IQueryService<Domain.District> queryDistrict { get; set; }
        public IQueryService<Domain.Contact> queryContact { get; set; }

        public OutpostOutputModel()
        {

            this.queryCountry = queryCountry;
            this.queryRegion = queryRegion;
            this.queryDistrict = queryDistrict;
            this.queryWarehouse = queryWarehouse;

            var Countries = new List<SelectListItem>();
            var Regions = new List<SelectListItem>();
            var Districts = new List<SelectListItem>();
            var Warehouses = new List<SelectListItem>();
        }

        public OutpostOutputModel(IQueryService<Domain.Country> queryCountry,
                                  IQueryService<Domain.Region> queryRegion,
                                  IQueryService<Domain.District> queryDistrict,
                                  IQueryService<Domain.Outpost> queryWarehouse,
                                  Guid? countryId,
                                  Guid? regionId,
                                  Guid? districtId)
        {


            this.queryCountry = queryCountry;
            this.queryRegion = queryRegion;
            this.queryDistrict = queryDistrict;
            this.queryWarehouse = queryWarehouse;
            this.queryContact = queryContact;

            var Countries = new List<SelectListItem>();
            var Regions = new List<SelectListItem>();
            var Districts = new List<SelectListItem>();
            var Warehouses = new List<SelectListItem>();

            Region = new RegionModel();
            District = new DistrictModel();
            Client = new ClientModel();
            Warehouse = new OutpostModel();

            this.Countries = Countries;
            this.Regions = Regions;
            this.Districts = Districts;
            this.Warehouse = Warehouse;

            var countries = queryCountry.Query().OrderBy(m => m.Name).ToList();


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

                var regions = queryRegion.Query().Where(it => it.Country.Id == countryId.Value).ToList();

                foreach (Domain.Region region in regions)
                {
                    if (regionId != null)
                    {
                        this.Regions.Add(new SelectListItem { Text = region.Name, Value = region.Id.ToString(), Selected = region.Id == regionId });
                    }
                    else
                    {
                        this.Regions.Add(new SelectListItem { Text = region.Name, Value = region.Id.ToString() });
                    }
                }

            }

            if (regionId != null)
            {
                if (this.Regions.Where(it => it.Value == regionId.Value.ToString()).ToList().Count > 0)
                    this.Regions.First(it => it.Value == regionId.Value.ToString()).Selected = true;

                var districts = queryDistrict.Query().Where(it => it.Region.Id == regionId.Value).ToList();

                foreach (Domain.District district in districts)
                {
                    if (regionId != null)
                    {
                        this.Districts.Add(new SelectListItem { Text = district.Name, Value = district.Id.ToString(), Selected = district.Id == districtId });
                    }
                    else
                    {
                        this.Districts.Add(new SelectListItem { Text = district.Name, Value = district.Id.ToString() });
                    }
                }

            }

            var resultOutposts = queryWarehouse.Query();
            if (resultOutposts != null)
            {
                var resultWarehouse = resultOutposts.Where(m => m.IsWarehouse);
                if (resultWarehouse != null)
                {
                    if (resultWarehouse.FirstOrDefault() != null)
                    {
                        var selectListItem = new SelectListItem();
                        selectListItem.Value = null;
                        selectListItem.Text = "Please select a Warehouse";
                        Warehouses.Add(selectListItem);
 
                        foreach (Domain.Outpost item3 in resultWarehouse)
                        {
                            selectListItem = new SelectListItem();

                            selectListItem.Value = item3.Id.ToString();
                            selectListItem.Text = item3.Name;
                            Warehouses.Add(selectListItem);
                        }
                    }
                }
            }
 

        }

    }
}