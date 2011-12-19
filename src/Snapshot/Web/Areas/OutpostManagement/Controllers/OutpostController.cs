using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
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
using Persistence.Queries.Regions;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class OutpostController : Controller
    {

        public OutpostModel OutpostModel { get; set; }
        public OutpostOutputModel OutpostModelOutput { get; set; }

        public IQueryOutposts QueryOutposts { get; set; }

        public IQueryService<Outpost> QueryService { get; set; }
        public IQueryService<MobilePhone> QueryMobilePhone { get; set; }
        public IQueryService<Country> QueryCountries { get; set; }
        public IQueryService<Region> QueryRegions { get; set; }
        public IQueryService<District> QueryDistricts { get; set; }
        public IQueryService<Client> QueryClients { get; set; }

        public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Outpost> DeleteCommand { get; set; }


        public OutpostOutputModel CreateOutpost { get; set; }

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview()
        {
            OutpostOverviewModel model = new OutpostOverviewModel();

            model.Outposts = new List<OutpostModel>();


            var queryResult = QueryService.Query();
            var countries = QueryCountries.Query();
            var regions = QueryRegions.Query();
            var districts = QueryDistricts.Query();

            CreateMappings();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    var outpostModelItem = new OutpostModel();
                    CreateMappings();
                    Mapper.Map(item, outpostModelItem);
                    model.Outposts.Add(outpostModelItem);
                });

            List<SelectListItem> Countries = new List<SelectListItem>();

              if (countries.ToList().Count > 0)
            {
                foreach (var item in countries)
                {
                    Countries.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }

              List<SelectListItem> Regions = new List<SelectListItem>();

              if (regions.ToList().Count > 0)
              {
                  foreach (var item in regions)
                  {
                      Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                  }
              }

              List<SelectListItem> Districts = new List<SelectListItem>();
              if (districts.ToList().Count > 0)
              {
                  foreach (var item in regions)
                  {
                      Districts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                  }
              }
              model.Countries = Countries;
              return View(model);

        }

        [HttpGet]
        public ActionResult PhoneListInput(Guid outpostId)
        {
            OverviewPhones model = new OverviewPhones();

            model.Items = new List<MobilePhoneModel>();

            var queryResult = QueryMobilePhone.Query().Where(m => m.Outpost_FK == outpostId);
            var queryService = QueryService.Query();
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

        //[HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create()
        {
            var model = CreateOutpost;


            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(OutpostInputModel outpostInputModel)
        {
            var model = new OutpostInputModel();

            if (!ModelState.IsValid)
            {
                return View("Create", CreateOutpost );
            }
            CreateMappings();
            var outpost = new Outpost();
            var client = QueryClients.Load(Client.DEFAULT_ID);
            outpost.Client = client;
            Mapper.Map(outpostInputModel, outpost);

            outpost.Country = QueryCountries.Load(outpostInputModel.Country.Id);
            outpost.Region = QueryRegions.Load(outpostInputModel.Region.Id);
            outpost.District = QueryDistricts.Load(outpostInputModel.District.Id);

            SaveOrUpdateCommand.Execute(outpost);
            return RedirectToAction("Overview", "Outpost");
        }

       

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid outpostId)
        {
            var _outpost = QueryService.Load(outpostId);
            OutpostOutputModel OutpostModel = new OutpostOutputModel(QueryCountries, QueryRegions, QueryDistricts);

            CreateMappings();
            Mapper.Map(_outpost, OutpostModel);

            return View(OutpostModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(OutpostInputModel outpostInputModel)
        {
            var model = new OutpostModel();

            if (!ModelState.IsValid)
            {
                return new EmptyResult();// View("Edit", outpostOutputModel);
            }

            CreateMappings();
            var _outpost = new Outpost();
            Mapper.Map(outpostInputModel, _outpost);

            _outpost.Country = QueryCountries.Load(outpostInputModel.Country.Id);
            _outpost.Region = QueryRegions.Load(outpostInputModel.Region.Id);
            _outpost.District = QueryDistricts.Load(outpostInputModel.District.Id);

            SaveOrUpdateCommand.Execute(_outpost);

            return RedirectToAction("Overview", "Outpost");
        }

        private static void CreateMappings()
        {
            Mapper.CreateMap<OutpostModel, Outpost>();
            Mapper.CreateMap<Outpost, OutpostModel>();

            Mapper.CreateMap<Outpost, OutpostInputModel>();
            Mapper.CreateMap<Outpost, OutpostOutputModel>()
                .ForMember("Region", m=> m.Ignore())
                .ForMember("District", m=> m.Ignore())
                .ForMember("Country", m=> m.Ignore());

                ;

            Mapper.CreateMap<OutpostInputModel, Outpost>()
                .ForMember("Region", m=> m.Ignore())
                .ForMember("District", m=> m.Ignore())
                .ForMember("Country", m=> m.Ignore());

            Mapper.CreateMap<OutpostOutputModel, Outpost>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<Region, RegionModel>();
            Mapper.CreateMap<RegionModel, Region>();

            Mapper.CreateMap<District, DistrictModel>();
            Mapper.CreateMap<DistrictModel, District>();

            Mapper.CreateMap<ClientModelOutpost, Client>();
            Mapper.CreateMap<Client, ClientModelOutpost>();

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
        public RedirectToRouteResult Delete(Guid outpostId)
        {
            var outpost = QueryService.Load(outpostId);

            if (outpost != null)
                DeleteCommand.Execute(outpost);

            return RedirectToAction("Overview", "Outpost");
        }


            var regions = QueryRegions.Query().Where(it => it.Country.Id == countryId.Value);

            if (regions.ToList().Count > 0)
            {
            var districts = QueryDistricts.Query().Where(m => m.Region.Id == regionId);
                {
            jr.Data = districts.Select(o => new { Value= o.Id, Text = o.Name });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Regions;

            //  ModelState.Add("Regions", ModelState.);
            return Json(Regions, JsonRequestBehavior.AllowGet);

        }

    }
}
