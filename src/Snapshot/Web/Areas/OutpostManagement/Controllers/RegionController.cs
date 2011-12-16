﻿using System;
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

        public ActionResult Overview()
        {
            RegionOverviewModel overviewModel = new RegionOverviewModel();
            RegionModel regionModel = new RegionModel();
            var regions = QueryRegion.GetAll().ToList();

            CreateMapping();

            foreach (Region item in regions)
            {
                regionModel = new RegionModel();
                Mapper.Map(item, regionModel);
                overviewModel.Regions.Add(regionModel);

            }
            overviewModel.Error = (string)TempData[TEMPDATA_ERROR_KEY];
            return View(overviewModel);
        }

        public ActionResult Create()
        {
            return View(RegionOutputModel);
        }

        [HttpPost]
        public ActionResult Create(RegionInputModel regionInputModel)
        {
            if (!ModelState.IsValid)
                return View("Create", RegionOutputModel);

            CreateMapping();
            var region = new Region();
            Mapper.Map(regionInputModel, region);

            var client = QueryClients.Load(Client.DEFAULT_ID);

            region.Client = client;

            SaveOrUpdateCommand.Execute(region);
            return RedirectToAction("Overview");
        }

        [HttpGet]
        public ViewResult Edit(Guid guid)
        {
            Region region = new Region();
            region = QueryService.Load(guid);
            CreateMapping();
            RegionOutputModel viewModel = new RegionOutputModel(QueryCountry);
            Mapper.Map(region, viewModel);


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
            
            SaveOrUpdateCommand.Execute(region);

            return RedirectToAction("Overview");
        }

        private RegionOutputModel MapDataFromInputModelToOutputModel(RegionInputModel regionInputModel)
        {
            var regionOutputModel = new RegionOutputModel(QueryCountry);
            regionOutputModel.Id = regionInputModel.Id;
            regionOutputModel.Name = regionInputModel.Name;
            regionOutputModel.Client = regionInputModel.Client;
            regionOutputModel.Country = regionInputModel.Country;
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
                    return RedirectToAction("Overview");
                }

                DeleteCommand.Execute(region);
            }

            return RedirectToAction("Overview");
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
