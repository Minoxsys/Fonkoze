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


namespace Web.Areas.OutpostManagement.Controllers
{
    public class RegionController : Controller
    {        
        public IQueryService<Region> QueryService { get; set; }
        
        public ISaveOrUpdateCommand<Region> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Region> DeleteCommand { get; set; }


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
            var regionInputModel = new RegionOutputModel();
            return View(regionInputModel);
        }

        [HttpPost]
        public ActionResult Create(RegionInputModel regionInputModel)
        {
             if (!ModelState.IsValid)
                return View("Create", new RegionOutputModel());

             CreateMapping();
            var region = new Region();
            Mapper.Map(regionInputModel, region);
            
            SaveOrUpdateCommand.Execute(region);
            return RedirectToAction("Overview");
        }

        [HttpGet]
        public ViewResult Edit(Guid guid)
        {
            var regionToEdit = QueryService.Load(guid);
            var viewModel = new RegionOutputModel()
            {
                Name = regionToEdit.Name,
                Id = regionToEdit.Id
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(RegionInputModel regionInputModel)
        {
            if (!ModelState.IsValid)
                return View("Edit", regionInputModel);

            var region = QueryService.Load(regionInputModel.Id);
            region.Name = regionInputModel.Name;

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
        }
    }
}
