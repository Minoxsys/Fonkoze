using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using System.Runtime.CompilerServices;
using Web.Areas.OutpostManagement.Models.Region;
using AutoMapper;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Country;
using Persistence.Queries;
using Core.Domain;

using Persistence.Queries.Regions;
using Web.Areas.OutpostManagement.Models.Client;


namespace Web.Areas.OutpostManagement.Controllers
{
    public class RegionController : Controller
    {
        public RegionOutputModel RegionOutputModel { get; set; }

        public RegionInputModel RegionInputModel { get; set; }

        public ISaveOrUpdateCommand<Region> SaveOrUpdateCommand { get; set; }

        public IQueryService<Region> QueryService { get; set; }

        public IDeleteCommand<Region> DeleteCommand { get; set; }

        public IQueryService<Country> QueryCountry { get; set; }

        public IQueryService<District> QueryDistrict { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryRegion QueryRegion { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";

        [HttpGet]
        public ActionResult Overview(Guid? countryId)
        {
            RegionOverviewModel overviewModel = new RegionOverviewModel(QueryCountry);
            List<Region> regions = new List<Region>();

            if (countryId != null)
            {
                if (countryId.Value != Guid.Empty)
                {
                    regions = QueryService.Query().Where<Region>(it => it.Country.Id == countryId.Value).ToList();
                }
                else
                {
                    Guid countryIdSelectedImplicit = new Guid();
                    if (overviewModel.Countries.Count > 0)
                    {
                        countryIdSelectedImplicit = Guid.Parse(overviewModel.Countries.First().Value);
                    }
                    regions = QueryService.Query().Where<Region>(it => it.Country.Id == countryIdSelectedImplicit).ToList();
                }
            }
            

            CreateMapping();

            foreach (Region item in regions)
            {
                var regionModel = new RegionModel();
                Mapper.Map(item, regionModel);
                regionModel.DistrictNo = QueryDistrict.Query().Count<District>(it => it.Region.Id == item.Id);
                regionModel.CountryId = item.Country.Id;
                overviewModel.Regions.Add(regionModel);
            }

            if (countryId != null)
            {
                var selectedCountry = overviewModel.Countries.Where<SelectListItem>(it => it.Value == countryId.Value.ToString()).ToList();
                    if(selectedCountry.Count > 0)
                        overviewModel.Countries.First<SelectListItem>(it => it.Value == countryId.Value.ToString()).Selected = true;
            }
            overviewModel.Error = (string)TempData[TEMPDATA_ERROR_KEY];
            return View(overviewModel);
        }
        public PartialViewResult OverviewTable(Guid? countryId)
        {
            var regionList = new List<RegionModel>();
            if (!countryId.HasValue)
                return PartialView(regionList);

            var regions = QueryService.Query().Where<Region>(it => it.Country.Id == countryId);

            foreach (Region item in regions)
            {
                CreateMapping();
                var regionModel = new RegionModel();
                Mapper.Map(item, regionModel);
                regionModel.DistrictNo = QueryDistrict.Query().Count<District>(it => it.Region.Id == item.Id);
                regionList.Add(regionModel);

            }
            return PartialView(regionList);
        }
        public ActionResult Create()
        {
            return View(RegionOutputModel);
        }

        [HttpPost]
        public ActionResult Create(RegionInputModel regionInputModel)
        {
            if (!ModelState.IsValid)
            {
                var regionOutputModel = MapDataFromInputModelToOutputModel(regionInputModel);
                return View("Create", regionOutputModel);
            }

            CreateMapping();
            var region = new Region();
            Mapper.Map(regionInputModel, region);

            var client = QueryClients.Load(Client.DEFAULT_ID);
            var country = QueryCountry.Load(regionInputModel.CountryId);

            region.Client = client;
            region.Country = country;

            SaveOrUpdateCommand.Execute(region);
            return RedirectToAction("Overview", new { countryId = regionInputModel.CountryId });
        }

        [HttpGet]
        public ViewResult Edit(Guid guid)
        {
            Region region = new Region();
            region = QueryService.Load(guid);
            CreateMapping();
            RegionOutputModel viewModel = new RegionOutputModel(QueryCountry);
            Mapper.Map(region, viewModel);

            viewModel.Countries.First<SelectListItem>(it => it.Value == region.Country.Id.ToString()).Selected = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(RegionInputModel regionInputModel)
        {
            if (!ModelState.IsValid)
            {
                var regionOutputModel = MapDataFromInputModelToOutputModel(regionInputModel);
                return View("Edit", regionOutputModel);
            }

            Region region = new Region();

            CreateMapping();
            Mapper.Map(regionInputModel, region);

            region.Country = QueryCountry.Load(regionInputModel.CountryId);
            SaveOrUpdateCommand.Execute(region);

            return RedirectToAction("Overview", new { countryId = region.Country.Id });
        }

        private RegionOutputModel MapDataFromInputModelToOutputModel(RegionInputModel regionInputModel)
        {
            
            var regionOutputModel = new RegionOutputModel(QueryCountry);
                        
            regionOutputModel.Id = regionInputModel.Id;
            regionOutputModel.Name = regionInputModel.Name;
            regionOutputModel.Client = regionInputModel.Client;
            regionOutputModel.CountryId = regionInputModel.CountryId;

            if (regionOutputModel.Countries.Count > 0)
            {
                var selectedCountry = regionOutputModel.Countries.Where(it => it.Value == regionInputModel.CountryId.ToString()).ToList();

                if (selectedCountry.Count > 0)
                    regionOutputModel.Countries.First(it => it.Value == regionInputModel.CountryId.ToString()).Selected = true;
            }
            return regionOutputModel;
        }

        [HttpPost]
        public RedirectToRouteResult Delete(Guid guid)
        {
            var region = QueryService.Load(guid);

            if (region != null)
            {
                var districtResults = QueryDistrict.Query().Where(it => it.Region.Id == region.Id);

                if (districtResults.ToList().Count != 0)
                {
                    TempData.Add("error", string.Format("The region {0} has districts associated, so it can not be deleted", region.Name));
                    return RedirectToAction("Overview",new { countryId = region.Country.Id });
                }

                DeleteCommand.Execute(region);
            }

            return RedirectToAction("Overview", new { countryId = region.Country.Id });
        }
        private void CreateMapping()
        {
            Mapper.CreateMap<RegionModel, Region>();
            Mapper.CreateMap<Region, RegionModel>();

            Mapper.CreateMap<Region, RegionInputModel>();
            Mapper.CreateMap<Region, RegionOutputModel>();

            Mapper.CreateMap<RegionInputModel, Region>();
            Mapper.CreateMap<RegionOutputModel, Region>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<ClientModel, Client>();
            Mapper.CreateMap<Client, ClientModel>();

        }


    }

}
