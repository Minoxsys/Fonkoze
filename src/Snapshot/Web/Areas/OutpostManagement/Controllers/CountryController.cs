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


        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<WorldCountryRecord> QueryWorldCountryRecords { get; set; }


        public ISaveOrUpdateCommand<Country> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Country> DeleteCommand { get; set; }

        public CountryOutputModel CountryOutputModel { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";
        private Client _client;
        private Core.Domain.User _user;

        public ActionResult Overview()
        {
            LoadUserAndClient();

            var countryOverviewModel = new CountryOverviewModel();

            var userSelectedCountries = this.QueryCountry.Query().Where(c=>c.Client == _client).Select(c=>c.Name).ToList();

            var worldRecords = (from worldRec in this.QueryWorldCountryRecords.Query()
                               where !userSelectedCountries.Contains(worldRec.Name)
                               select worldRec).ToList();
             

            countryOverviewModel.WorldRecords = new JavaScriptSerializer().Serialize(worldRecords);

            return View(countryOverviewModel);
        }

        [HttpGet]
        public JsonResult Index(CountryIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value - indexModel.start.Value;

            var countryDataQuery = this.QueryCountry.Query()
                .Where( c=>c.Client.Id == _client.Id);


            if (indexModel.dir == "ASC")
            {
                countryDataQuery = countryDataQuery.OrderBy(c => c.Name);
            }
            else
            {
                countryDataQuery = countryDataQuery.OrderByDescending(c => c.Name);
            }

                
            countryDataQuery = countryDataQuery.Take(pageSize)
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
            LoadUserAndClient();

            var country = new Country{
                Client = this._client,
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

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = LoadClient.Load(clientId);
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


    }
}
