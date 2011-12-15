using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Outpost;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Validation.ValidDate;
using System.Globalization;
using Domain;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class OutpostController : Controller
    {
        public IQueryService<Outpost> QueryService { get; set; }

        public IQueryService<MobilePhone> QueryMobilePhone { get; set; }
        public IQueryService<Country> QueryCountries { get; set; }
        public IQueryService<Region> QueryRegions { get; set; }
        public IQueryService<Outpost> QueryDistricts { get; set; }
 
        public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Outpost> DeleteCommand { get; set; }

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview()
        {
            Web.Areas.OutpostManagement.Models.Outpost.Overview model = new Web.Areas.OutpostManagement.Models.Outpost.Overview();

            model.Items = new List<OutpostModel>();

            var queryResult = QueryService.Query();
            var queryCountries = QueryCountries.Query();
            var queryRegions = QueryRegions.Query();
            var queryDistricts = QueryDistricts.Query();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    OutpostModel viewModelItem = new OutpostModel();
                    CreateMappings();
                    Mapper.Map(item, viewModelItem);
                    model.Items.Add(viewModelItem);
                });

            var countries = from country in queryCountries select country;
            var regions = from region in queryRegions select region;
            var districts = from district in queryDistricts select district;
            //model.Countries = (List<Country>) queryCountries;

            model.Countries = new List<Domain.Country>();
            foreach (Country country in countries)
            {
                model.Countries.Add(country);
            }
            model.Regions = new List<Domain.Region>();
            foreach (Region region in regions)
            {
                model.Regions.Add(region);
            }
            model.Districts = new List<Domain.District>();
            //foreach (District district in districts)
            //{
            //    model.Countries.Add(district);
            //}

             return View(model);
        }

        [HttpGet]
        public ActionResult PhoneListInput(Guid outpostId)
        {
            Web.Areas.OutpostManagement.Models.Outpost.OverviewPhones model = new Web.Areas.OutpostManagement.Models.Outpost.OverviewPhones();

            model.Items = new List<MobilePhoneModel>();
            
            var queryResult = QueryMobilePhone.Query();
            var queryOutposts = QueryService.Query();
            var queryResultCountries = QueryCountries.Query();
            var queryResultRegion = QueryRegions.Query();
            var queryResultCDistrict = QueryDistricts.Query();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    MobilePhoneModel viewModelItem = new MobilePhoneModel();
                    CreateMappingsPhones();
                    Mapper.Map(item, viewModelItem);
                    model.Items.Add(viewModelItem);
                });

            return View(model);
        }

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create()
        {
            var model = new OutpostModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(OutpostModel outpostModel)
        {
            var model = new OutpostModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var outpost = new Outpost();
            Mapper.Map(outpostModel, outpost);

            SaveOrUpdateCommand.Execute(outpost);

            return RedirectToAction("Overview", "Outpost");
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid outpost)
        {
            var _outpost = QueryService.Load(outpost);
            var OutpostModel = new OutpostModel();

            CreateMappings();
            Mapper.Map(_outpost, OutpostModel);

            return View(OutpostModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(OutpostModel outpostModel)
        {
            var model = new OutpostModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var _outpost = new Outpost();
            Mapper.Map(outpostModel, _outpost);

            SaveOrUpdateCommand.Execute(_outpost);

            return RedirectToAction("Overview", "Outpost");
        }

        private static void CreateMappings(Outpost entity = null)
        {
            Mapper.CreateMap<Outpost, OutpostModel>();

            var mapOutpost = Mapper.CreateMap<OutpostModel, Outpost>();

            if (entity != null)
                mapOutpost.ForMember(m => m.Id, options => options.Ignore());
        }

        private static void CreateMappingsPhones(MobilePhone entity = null)
        {
            Mapper.CreateMap<MobilePhone, MobilePhoneModel>();

            var mapMobilePhone = Mapper.CreateMap<MobilePhoneModel, MobilePhone>();

            if (entity != null)
                mapMobilePhone.ForMember(m => m.Id, options => options.Ignore());
        }

        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid countryId)
        {
            var country = QueryService.Load(countryId);

            if (country != null)
                DeleteCommand.Execute(country);

            return RedirectToAction("Overview", "Outpost");
        }

    }
}
