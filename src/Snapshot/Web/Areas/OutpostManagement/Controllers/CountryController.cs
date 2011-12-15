using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Country;
using Microsoft.Practices.Unity;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Validation.ValidDate;
using System.Globalization;
using PagedList;
using Domain;


namespace Web.Areas.OutpostManagement.Controllers
{
    public class CountryController : Controller
    {
        public IQueryService<Country> QueryService { get; set; }
        public IQueryService<Client> QueryClient { get; set; }

        public ISaveOrUpdateCommand<Country> SaveOrUpdateCommand { get; set; }

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
            var model = new CountryModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(CountryModel countryModel)
        {
            var model = new CountryModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var country = new Country();
            Mapper.Map(countryModel, country);
            country.Client = QueryClient.Load(Guid.Empty); // hardcoded client id value

            SaveOrUpdateCommand.Execute(country);

            return RedirectToAction("Overview", "Country");
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid countryId)
        {
            var country = QueryService.Load(countryId);
            var countryModel = new CountryModel();

            CreateMappings();
            Mapper.Map(country, countryModel);

            return View(countryModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(CountryModel countryModel)
        {
            var model = new CountryModel();

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

        private static void CreateMappings(Country entity = null)
        {
            Mapper.CreateMap<Country, CountryModel>();

            var mapCountry = Mapper.CreateMap<CountryModel, Country>();

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
