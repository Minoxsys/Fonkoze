using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Validation.ValidDate;
using System.Globalization;
using Core.Domain;
using Persistence.Queries.Districts;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.Country;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class DistrictController : Controller
    {
        public ISaveOrUpdateCommand<District> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<District> DeleteCommand { get; set; }

        public IQueryService<Region> QueryRegion { get; set; }

        public IQueryService<Outpost> QueryOutpost { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Country> QueryCountry { get; set; }

        public IQueryDistrict QueryDistrict { get; set; }

        public IQueryService<District> QueryService { get; set; }

        public DistrictOutputModel DistrictOutputModel { get; set; }

        public DistrictInputModel DistrictInputModel { get; set; }
       
        private const string TEMPDATA_ERROR_KEY = "error";

        public ActionResult Overview()
        {
            DistrictOverviewModel overviewModel = new DistrictOverviewModel();
            DistrictModel districtModel = new DistrictModel();
              var districts = QueryDistrict.GetAll().ToList();

            

            foreach (District item in districts)
            {
                CreateMapping();
                districtModel = new DistrictModel();
                Mapper.Map(item, districtModel);
                overviewModel.Districts.Add(districtModel);

            }
            overviewModel.Error = (string)TempData[TEMPDATA_ERROR_KEY];
            return View(overviewModel);
        }

        private void CreateMapping()
        {
            Mapper.CreateMap<DistrictModel, District>();
            Mapper.CreateMap<District, DistrictModel>();

            Mapper.CreateMap<DistrictInputModel, District>();
            Mapper.CreateMap<DistrictOutputModel, District>();

            Mapper.CreateMap<ClientModel, Client>();
            Mapper.CreateMap<RegionModel, Region>();

            Mapper.CreateMap<Region, RegionModel>();
            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();
            Mapper.CreateMap<Client, ClientModel>();

            Mapper.CreateMap<District, DistrictOutputModel>();
            Mapper.CreateMap<District, DistrictInputModel>();
        }

        public ViewResult Create()
        {
            return View(DistrictOutputModel);
        }

        [HttpPost]
        public ActionResult Create(DistrictInputModel districtInputModel)
        {
            if (!ModelState.IsValid)
                return View("Create", DistrictOutputModel);

            CreateMapping();
            var district = new District();
            Mapper.Map(districtInputModel, district);

            var client = QueryClients.Load(Client.DEFAULT_ID);

            district.Client = client;

            SaveOrUpdateCommand.Execute(district);
            return RedirectToAction("Overview");
        }

        public ViewResult Edit(Guid guid)
        {
            District district = new District();
            district = QueryService.Load(guid);
            CreateMapping();
            DistrictOutputModel viewModel = new DistrictOutputModel(QueryCountry);
            Mapper.Map(district, viewModel);


            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(DistrictInputModel districtInputModel)
        {
            if (!ModelState.IsValid)
            {
                var districtOutputModel = MapDatFromInputModelToOutputModel(districtInputModel);
                
                return View("Edit", districtOutputModel);
            }

            District region = new District();

            CreateMapping();
            Mapper.Map(districtInputModel, region);

            SaveOrUpdateCommand.Execute(region);

            return RedirectToAction("Overview");
        }

        public ActionResult GetRegionsForCountry(string countryName)
        {
            this.DistrictOutputModel = new Models.District.DistrictOutputModel(QueryCountry);

            var regions = QueryRegion.Query().Where(it => it.Country.Name == countryName);

            if (regions.ToList().Count > 0)
            {
                foreach (var item in regions)
                {
                    DistrictOutputModel.Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                } 
            }

            return RedirectToAction("Create", DistrictOutputModel);
        }
        private DistrictOutputModel MapDatFromInputModelToOutputModel(DistrictInputModel districtInputModel)
        {
            var districtOutputModel = new DistrictOutputModel(QueryCountry);
            districtOutputModel.Client = districtInputModel.Client;
            districtOutputModel.Id = districtInputModel.Id;
            districtOutputModel.Name = districtInputModel.Name;
            districtOutputModel.Region = districtInputModel.Region;
            return districtOutputModel;
        }

        [HttpPost]
        public ActionResult Delete(Guid guid)
        {
            var district = QueryService.Load(guid);

            if (district != null)
            {
                var districtResults = QueryOutpost.Query().Where(it => it.District.Id == district.Id);

                if (districtResults.ToList().Count != 0)
                {
                    TempData.Add("error", string.Format("The district {0} has outposts associated, so it can not be deleted", district.Name));
                    return RedirectToAction("Overview");
                }

                DeleteCommand.Execute(district);
            }

            return RedirectToAction("Overview");
        }
    }
}
