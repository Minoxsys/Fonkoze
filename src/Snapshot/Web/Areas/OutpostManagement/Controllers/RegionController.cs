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
using MvcPaging;


namespace Web.Areas.OutpostManagement.Controllers
{
    public class RegionController : Controller
    {
        public RegionOutputModel RegionOutputModel { get; set; }

        public RegionInputModel RegionInputModel { get; set; }

        public IQueryService<Region> QueryService { get; set; }

        public ISaveOrUpdateCommand<Region> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Region> DeleteCommand { get; set; }

        public IQueryService<Country> QueryCountry { get; set; }

        public ActionResult Overview()
        {
            RegionOverviewModel overviewModel = new RegionOverviewModel();
            RegionModel regionModel = new RegionModel();
            var regions = QueryService.Query().ToList();


            CreateMapping();

            foreach (Region item in regions)
            {
                regionModel = new RegionModel();
                Mapper.Map(item, regionModel);
                overviewModel.Regions.Add(regionModel);

            }

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
                return View("Edit", regionInputModel);

            Region region = new Region();

            CreateMapping();
            Mapper.Map(regionInputModel, region);
            SaveOrUpdateCommand.Execute(region);

            return RedirectToAction("Overview");
        }

        [HttpPost]
        public RedirectToRouteResult Delete(Guid guid)
        {
            var entity = QueryService.Load(guid);

            if (entity != null)
                DeleteCommand.Execute(entity);

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

        }


    }

}
