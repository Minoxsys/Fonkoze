using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
        

        public int PageSize = 8;

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview(int page)
        {
            var loggedUser = this.User.Identity.Name.ToString();
            var currentUser = QueryUsers.Query().Where(m => m.UserName == loggedUser).FirstOrDefault();
            var currentClient = currentUser.ClientId;

            var queryResult = QueryCountry.Query();

            var paginatedCountries =
                  queryResult
                              .OrderBy(p => p.Name)
                              .Skip((page - 1)*PageSize)
                              .Take(PageSize);

            CountryOverviewModel model =
                new CountryOverviewModel
                {
                    Countries = null,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = queryResult.Count() 
                    }, 
                    Error = ""
                };

            model.Countries = new List<CountryModel>();


            if (paginatedCountries.ToList().Count() > 0)
                paginatedCountries.ToList().ForEach(item =>
                {
                    var viewModelItem = new CountryModel();
                    CreateMappings();
                    Mapper.Map(item, viewModelItem);
                    model.Countries.Add(viewModelItem);
                });

            model.Error = (string)TempData[TEMPDATA_ERROR_KEY];

            return View(model);
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
        public ActionResult Create(CountryInputModel countryModel)
        {
            var model = new CountryOutputModel();

            if (!ModelState.IsValid)
            {
                var countryOutputModel1 = MapDataFromInputModelToOutputModel(countryModel);
                return View("Create", countryOutputModel1);
            }

            var loggedUser = this.User.Identity.Name.ToString();
            var currentUser = QueryUsers.Query().Where(m => m.UserName == loggedUser).FirstOrDefault();
            var currentClient = currentUser.ClientId;

            CreateMappings();
            var country = new Country();
            Mapper.Map(countryModel, country);
            country.Client = QueryClients.Load(currentClient); 
            var doubleCountry = QueryCountry.Query();

            if (doubleCountry != null)
            {
                doubleCountry = doubleCountry.Where(m => m.Name == country.Name);

                if (doubleCountry.ToList().Count() > 0)
                {
                    TempData.Add("error", string.Format("The country {0} is already added, so it can not be re-inserted", country.Name));
                    return RedirectToAction("Overview", "Country", new { page = 1 });
                }
            }
            SaveOrUpdateCommand.Execute(country);

            return RedirectToAction("Overview", "Country", new { page = 1});
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
            return RedirectToAction("Overview", new { page = 1});
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
    }
}
