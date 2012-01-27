using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Country;
using AutoMapper;
using Core.Persistence;
using Core.Domain;
using Domain;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class CountryController : Controller
    {

        private const string TEMPDATA_ERROR_KEY = "error";

        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public ISaveOrUpdateCommand<Country> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Country> DeleteCommand { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public CountryOutputModel CountryOutputModel { get; set; }


        public int PageSize = 50;

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview()
        {
            var countryOverviewModel = new CountryOverviewModel();

            var worldRecords = this.QueryWorldCountryRecords.Query().ToList();

            countryOverviewModel.WorldRecords = new JavaScriptSerializer().Serialize(worldRecords);

            return View(countryOverviewModel);
        }

        [HttpGet]
        public JsonResult Index(CountryIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value - indexModel.start.Value;

            var countryDataQuery = this.QueryCountry.Query()
                .Where( c=>c.Client.Id == Client.DEFAULT_ID)
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var totalItems = countryDataQuery.Count();

            var countryModelListProjection = (from countryData in countryDataQuery.ToList()
                         select new CountryModel
                         {
                             Id = countryData.Id,
                             ISOCode = countryData.ISOCode,
                             Name = countryData.Name,
                             PhonePrefix = countryData.PhonePrefix,
                         }).ToArray();

            return Json(new CountryIndexOutputModel
            {
                Countries = countryModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create(int page)
        {
            return View(CountryOutputModel);
        }


        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public EmptyResult Create(CountryInputModel countryModel)
        {
            var model = new CountryOutputModel();
            var loggedUser = User.Identity.Name;
            var currentUser = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);
            var currentClientId = currentUser.ClientId;

            var country = new Country{
                Client = QueryClients.Load(currentClientId),
                Name = countryModel.Name,
                ISOCode = countryModel.ISOCode,
                PhonePrefix = countryModel.PhonePrefix
            };

            SaveOrUpdateCommand.Execute(country);

            return new EmptyResult();
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid countryId)
        {
            var country = QueryCountry.Load(countryId);
            var countryModel = new CountryOutputModel();

            CreateMappings();
            Mapper.Map(country, countryModel);

            return View(countryModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(CountryInputModel countryInputModel)
        {
            if (!ModelState.IsValid)
            {
                var countryOutputModel1 = MapDataFromInputModelToOutputModel(countryInputModel);
                return View("Edit", countryOutputModel1);
            }


            Country region = new Country();
            CreateMappings();
            var country = new Country();
            Mapper.Map(countryInputModel, country);

            SaveOrUpdateCommand.Execute(country);

            return RedirectToAction("Overview", "Country", new { page = 1 });
        }

        private static void CreateMappings(Country entity = null)
        {
            Mapper.CreateMap<CountryModel, Country>();
            Mapper.CreateMap<Country, CountryModel>();

            Mapper.CreateMap<Country, CountryInputModel>();
            Mapper.CreateMap<Country, CountryOutputModel>();

            Mapper.CreateMap<CountryInputModel, Country>();
            Mapper.CreateMap<CountryOutputModel, Country>();

            Mapper.CreateMap<ClientModel, Client>();
            Mapper.CreateMap<Client, ClientModel>();
        }


        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid countryId)
        {
            var country = QueryCountry.Load(countryId);
            var regionResults = QueryRegion.Query();

            if (regionResults != null)
            {
                if (country != null)
                {
                    regionResults = regionResults.Where(it => it.Country.Name == country.Name);

                    if (regionResults.ToList().Count != 0)
                    {
                        TempData.Add("error", string.Format("The Country {0} has regions associated, so it can not be deleted", country.Name));
                        return RedirectToAction("Overview", new { page = 1 });
                    }
                }

            }

            DeleteCommand.Execute(country);
            return RedirectToAction("Overview", new { page = 1 });
        }


        private CountryOutputModel MapDataFromInputModelToOutputModel(CountryInputModel countryInputModel)
        {

            var countryOutputModel = new CountryOutputModel();

            countryOutputModel.Id = countryInputModel.Id;
            countryOutputModel.Name = countryInputModel.Name;
            countryOutputModel.Client = countryInputModel.Client;
            countryOutputModel.ISOCode = countryInputModel.ISOCode;
            countryOutputModel.PhonePrefix = countryInputModel.PhonePrefix;
            return countryOutputModel;
        }


        public object countryOutputModel { get; set; }

        public IQueryService<WorldCountryRecord> QueryWorldCountryRecords { get; set; }
    }
}
