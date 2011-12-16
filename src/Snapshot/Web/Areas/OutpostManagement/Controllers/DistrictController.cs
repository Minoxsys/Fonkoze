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

using Persistence.Queries.Districts;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Client;

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

        public ActionResult Overview(Guid? countryId, Guid? regionId)
        {
            DistrictOverviewModel overviewModel;
            IQueryable<District> districts;
            IQueryable<Region> regions;
            IQueryable<Country> countries;

            //when user gets here without knowing country or region
            if ((countryId == null) && (regionId == null))
            {
                overviewModel = new DistrictOverviewModel(QueryCountry, QueryRegion);
                Guid regionSelectedId = new Guid();

                if (overviewModel.Regions.Count > 0)
                {
                    regionSelectedId = Guid.Parse(overviewModel.Regions.First().Value);
                }
                districts = QueryService.Query().Where(it => it.Region.Id == regionSelectedId);
             }
            else
            {
                countries = QueryCountry.Query();
                regions = QueryRegion.Query().Where<Region>(it => it.Country.Id == countryId.Value);
                districts = QueryService.Query().Where<District>(it => it.Region.Id == regionId.Value);

                overviewModel = new DistrictOverviewModel();

                foreach (Country item in countries)
                {
                    overviewModel.Countries.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }

                foreach (Region item in regions)
                {
                    overviewModel.Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                if (overviewModel.Countries.Count > 0)
                {
                    var selectedCountry = overviewModel.Countries.First<SelectListItem>(it => it.Value == countryId.Value.ToString());
                        if(selectedCountry!=null)
                            selectedCountry.Selected = true;
                }

                var regionsWithRegionId = overviewModel.Regions.Where<SelectListItem>(it => it.Value == regionId.Value.ToString()).ToList();
                if (regionsWithRegionId.Count > 0)
                    regionsWithRegionId[0].Selected = true;

            }

            if (districts.ToList().Count != 0)
            {
                foreach (District item in districts)
                {
                    CreateMapping();
                    var districtModel = new DistrictModel();
                    Mapper.Map(item, districtModel);
                    districtModel.OutpostNo = 0;//= QueryOutpost.Query().Count<Outpost>(it => it.District.Id == item.Id);
                    overviewModel.Districts.Add(districtModel);

                }
            }
            
            
            overviewModel.Error = (string)TempData[TEMPDATA_ERROR_KEY];
            return View(overviewModel);
        }

        public PartialViewResult OverviewTable(Guid? regionId)
        {
            var districtList = new List<DistrictModel>();
            if (!regionId.HasValue)
                return PartialView(districtList);

            var districts = QueryService.Query().Where<District>(it => it.Region.Id == regionId.Value);

            foreach (District item in districts)
            {
                CreateMapping();
                var districtModel = new DistrictModel();
                Mapper.Map(item, districtModel);
                districtModel.OutpostNo = QueryOutpost.Query().Count<Outpost>(it => it.District.Id == item.Id);
                districtList.Add(districtModel);

            }
            return PartialView(districtList);
        }

        private void CreateMapping()
        {
            Mapper.CreateMap<DistrictModel, District>();
            Mapper.CreateMap<District, DistrictModel>();

            Mapper.CreateMap<DistrictInputModel, District>().ForMember("Region",
                m=>m.Ignore());
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
            var region = QueryRegion.Load(districtInputModel.Region.Id);
            var country = QueryCountry.Load(districtInputModel.Region.CountryId);
            district.Client = client;
            district.Region = region;
            district.Region.Country = country;

            SaveOrUpdateCommand.Execute(district);
            return RedirectToAction("Overview");
        }

        public ViewResult Edit(Guid guid)
        {
            District district = new District();
            district = QueryService.Load(guid);
            CreateMapping();
            DistrictOutputModel viewModel = new DistrictOutputModel(QueryCountry, QueryRegion);
            Mapper.Map(district, viewModel);

            viewModel.Countries.Where<SelectListItem>(it => it.Value == district.Region.Country.Id.ToString()).ToList()[0].Selected = true;

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

            District district = new District();

            CreateMapping();
            Mapper.Map(districtInputModel, district);

            var region = QueryRegion.Load(districtInputModel.Region.Id);

            district.Region = region;

            SaveOrUpdateCommand.Execute(district);

            return RedirectToAction("Overview");
        }

        [HttpGet]
        public JsonResult GetRegionsForCountry(Guid? countryId)
        {
            List<SelectListItem> Regions = new List<SelectListItem>();

            var regions = QueryRegion.Query().Where(it => it.Country.Id == countryId.Value);

            if (regions.ToList().Count > 0)
            {
                foreach (var item in regions)
                {
                    Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Regions;

            //  ModelState.Add("Regions", ModelState.);
            return Json(Regions, JsonRequestBehavior.AllowGet);

        }
        private DistrictOutputModel MapDatFromInputModelToOutputModel(DistrictInputModel districtInputModel)
        {
            var districtOutputModel = new DistrictOutputModel(QueryCountry, QueryRegion);
            districtOutputModel.Client = new ClientModel {
                Id = Client.DEFAULT_ID
            }; 
            districtOutputModel.Id = districtInputModel.Id;
            districtOutputModel.Name = districtInputModel.Name;

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
