﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
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

namespace Web.Areas.OutpostManagement.Controllers
{
    public class CountryController : Controller
    {
        //[Dependency]
        public IQueryService<Country> QueryService { get; set; }

        //[Dependency]
        public IQueryService<Region> QueryRegions { get; set; }

        //[Dependency]
        public IQueryService<District> QueryDistricts { get; set; }

        //[Dependency]
        public IQueryService<Outpost> QueryOutposts { get; set; }

        //[Dependency]
        public ISaveOrUpdateCommand<Country> SaveOrUpdateCommand { get; set; }

        //[Dependency]
        public IDeleteCommand<Country> DeleteCommand { get; set; }

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview()
        {
            Overview model = new Overview();

            model.Items = new List<CountryModel>();

            var queryResult = QueryService.Query();

            if (queryResult.ToList().Count()  > 0)
                queryResult.ToList().ForEach(item =>
                {
                    var viewModelItem = new CountryModel();

                    CreateMappings();

                    Mapper.Map(item, viewModelItem);

                    model.Items.Add(viewModelItem);
                });

            return View(model);
        }

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create()
        {
            var model = new CountryModelOutput();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(CountryModelInput countryModel)
        {
            var model = new CountryModelInput();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var country = new Country();
            Mapper.Map(countryModel, country);

            SaveOrUpdateCommand.Execute(country);

            return RedirectToAction("Overview", "Country");
        }
     

        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ViewResult Edit(Guid countryId)
        {
            var country = QueryService.Load(countryId);
            var countryModel = new CountryModel();

            CreateMappings();
            Mapper.Map(country, countryModel);

            return View(countryModel);
        }

 
        private static void CreateMappings(Country entity = null)
        {
            Mapper.CreateMap<Country, CountryModel>();

            var mapCountry = Mapper.CreateMap<CountryModelInput, Country>();

            if (entity != null)
                mapCountry.ForMember(m => m.Id, options => options.Ignore());
        }


        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid countryId)
        {
            var country = QueryService.Load(countryId);

            if (country != null)
                DeleteCommand.Execute(country);

            return RedirectToAction("Overview", "Country");
        }

    }
}
