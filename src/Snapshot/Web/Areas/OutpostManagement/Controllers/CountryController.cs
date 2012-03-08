using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Core.Domain;
using Core.Persistence;
using Domain;
using FluentNHibernate.Conventions;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Models.Shared;
using NHibernate.Linq;
using Web.Security;
using Core.Security;

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

        public IPermissionsService PermissionService { get; set; }

        private const String COUNTRY_ADD_PERMISSION = "Country.Edit";
        private const String COUNTRY_DELETE_PERMISSION = "Country.Delete";

        private Client _client;
        private User _user;

        [Requires(Permissions = "Country.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(COUNTRY_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(COUNTRY_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           

            CountryOverviewModel countryOverviewModel = new CountryOverviewModel();

            List<WorldCountryRecord> worldRecords = GetAvailableWorldRecords();
             
            countryOverviewModel.WorldRecords = new JavaScriptSerializer().Serialize(worldRecords);

            return View(countryOverviewModel);
        }

        public ActionResult WorldRecords()
        {
            var worldRecord = GetAvailableWorldRecords();
            return Json(worldRecord, JsonRequestBehavior.AllowGet);
        }
  
        private List<WorldCountryRecord> GetAvailableWorldRecords()
        {
            LoadUserAndClient();

            var userSelectedCountries = this.QueryCountry.Query().Where(c => c.Client == _client).Select(c => c.Name).ToList();

            var worldRecords = (from worldRec in this.QueryWorldCountryRecords.Query() where !userSelectedCountries.Contains(worldRec.Name) orderby worldRec.Name select worldRec ).ToList();

            return worldRecords;
        }

        [HttpGet]
        public JsonResult Index(CountryIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;

            var countryDataQuery = this.QueryCountry.Query()
            .Where(c => c.Client.Id == _client.Id);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Country>>>()
            {
                { "Name-ASC", () => countryDataQuery.OrderBy(c => c.Name) },
                { "Name-DESC", () => countryDataQuery.OrderByDescending(c => c.Name) },
                { "ISOCode-ASC", () => countryDataQuery.OrderBy(c => c.ISOCode) },
                { "ISOCode-DESC", () => countryDataQuery.OrderByDescending(c => c.ISOCode) },
                { "PhonePrefix-ASC", () => countryDataQuery.OrderBy(c => c.PhonePrefix) },
                { "PhonePrefix-DESC", () => countryDataQuery.OrderByDescending(c => c.PhonePrefix) }
            };

			Func<IQueryable<Country>> orderCountries;
			if (orderByColumnDirection.TryGetValue(String.Format("{0}-{1}", indexModel.sort, indexModel.dir), out orderCountries))
			{
				countryDataQuery = orderCountries.Invoke();
			}

            var totalItems = countryDataQuery.Count();
            countryDataQuery = countryDataQuery.Take(pageSize)
                                               .Skip(indexModel.start.Value);

            var countryModelListProjection = (from countryData in countryDataQuery.ToList() select new CountryModel
            {
                Id = countryData.Id,
                ISOCode = countryData.ISOCode,
                Name = countryData.Name,
                PhonePrefix = countryData.PhonePrefix
            }).ToArray();

            return Json(new CountryIndexOutputModel
            {
                Countries = countryModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public EmptyResult Create(CountryInputModel countryModel)
        {
            LoadUserAndClient();

            var country = new Country
            {
                Client = this._client,
                Name = countryModel.Name,
                ISOCode = countryModel.ISOCode,
                PhonePrefix = countryModel.PhonePrefix
            };

            SaveOrUpdateCommand.Execute(country);

            return new EmptyResult();
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = LoadClient.Load(clientId);
        }

        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public JsonResult Delete(Guid? countryId)
        {
            if (countryId.HasValue == false)
            {
                return Json(new JsonActionResponse
                {
                    Status = "Error",
                    Message = "You must supply a countryId in order to remove the country"
                });
            }
            var country = QueryCountry.Load(countryId.Value);
            var regionResults = QueryRegion.Query();

            if (regionResults != null)
            {
                if (country != null)
                {
                    var regionsForCountry = regionResults.Where(it => it.Country.Name == country.Name).ToList();

                    if (regionsForCountry.Count != 0)
                    {
                        return Json(new JsonActionResponse
                        {
                            Status = "Error",
                            Message = String.Format("Country {0} has {1} region(s) associated, and can not be removed.", country.Name, regionsForCountry.Count)
                        }); 
                    }
                }
            }

            DeleteCommand.Execute(country);
            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Country {0} was removed.", country.Name)
                });
        }
    }
}