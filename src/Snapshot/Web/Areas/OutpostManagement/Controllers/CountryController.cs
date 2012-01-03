using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Country;
using AutoMapper;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class CountryController : Controller
    {

        private const string TEMPDATA_ERROR_KEY = "error";

        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Client> QueryClient { get; set; }

        public ISaveOrUpdateCommand<Country> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Country> DeleteCommand { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }

        public int PageSize = 8;

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview(int page)
        {
            var queryResult = QueryCountry.Query();

            var paginatedCountries = 
                  QueryCountry.Query()
                              .OrderBy(p => p.Name)
                              .Skip((page - 1)*PageSize)
                              .Take(PageSize);

            Overview model = 
                new Overview { Countries = null,
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
            var model = new CountryOutputModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(CountryOutputModel countryModel)
        {
            var model = new CountryModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
  

            CreateMappings();
            var country = new Country();
            Mapper.Map(countryModel, country);
            country.Client = QueryClient.Load(Client.DEFAULT_ID); // hardcoded client id value
            var doubleCountry = QueryCountry.Query().Where(m => m.Name == country.Name);

          if (doubleCountry.ToList().Count() > 0)
          {
              TempData.Add("error", string.Format("The country {0} is already added, so it can not be re-inserted", country.Name));
              return RedirectToAction("Overview", "Country", new { page = 1 });
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

            if (country != null)
            {
                var regionResults = QueryRegion.Query().Where(it => it.Country.Id == country.Id);

                if (regionResults.ToList().Count != 0)
                {
                    TempData.Add("error", string.Format("The Country {0} has regions associated, so it can not be deleted", country.Name));
                    return RedirectToAction("Overview", new { page = 1});
                }

                DeleteCommand.Execute(country);
            } 
            
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
