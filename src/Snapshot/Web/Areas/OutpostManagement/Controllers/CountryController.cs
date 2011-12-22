using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Domain;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Region;
using AutoMapper;
using Core.Persistence;
using Domain;
using PagedList;
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

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview()
        {
            Overview model = new Overview();

            model.Items = new List<CountryModel>();

            var queryResult = QueryCountry.Query();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    var viewModelItem = new CountryModel();

                    CreateMappings();

                    Mapper.Map(item, viewModelItem);

                    model.Items.Add(viewModelItem);
                });

            model.Error = (string)TempData[TEMPDATA_ERROR_KEY];

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
            country.Client = QueryClient.Load(Client.DEFAULT_ID); // hardcoded client id value

            SaveOrUpdateCommand.Execute(country);

            return RedirectToAction("Overview", "Country");
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid countryId)
        {
            var country = QueryCountry.Load(countryId);
            var countryModel = new CountryModel();

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
                //var countryOutputModel = MapDataFromInputModelToOutputModel(countryInputModel);
                //return View("Edit", countryOutputModel);
            }


            Country region = new Country();
            CreateMappings();
            var country = new Country();
            Mapper.Map(countryOutputModel, country);

            SaveOrUpdateCommand.Execute(country);

            return RedirectToAction("Overview", "Country");
        }

        private static void CreateMappings(Country entity = null)
        {
            Mapper.CreateMap<CountryModel, Country>();
            Mapper.CreateMap<Country, CountryModel>();

            Mapper.CreateMap<Country, RegionInputModel>();
            Mapper.CreateMap<Country, RegionOutputModel>();

            Mapper.CreateMap<CountryInputModel, Country>();
            Mapper.CreateMap<CountryOutputModel, Country>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

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
                    return RedirectToAction("Overview");
                }

                DeleteCommand.Execute(country);
            } 
            
            return RedirectToAction("Overview");
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
