using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Persistence.Queries.Outposts;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using AutoMapper;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class OutpostController : Controller
    {

        public OutpostModel OutpostModel { get; set; }
        public OutpostOutputModel OutpostModelOutput { get; set; }

        public IQueryOutposts QueryOutposts { get; set; }

        public IQueryService<Outpost> QueryService { get; set; }
        public IQueryService<Contact> QueryMobilePhone { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }

        public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Outpost> DeleteCommand { get; set; }

        public OutpostOutputModel OutpostOutputModel { get; set; }

        public OutpostInputModel OutpostInputModel { get; set; }

        public OutpostOutputModel CreateOutpost { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";


        //[Requires(Permissions = "Country.Overview")]
        [HttpGet]
        public ActionResult Overview(Guid? countryId, Guid? regionId, Guid? districtId)
        {
            OutpostOverviewModel model;
            IQueryable<Outpost> outposts;
            IQueryable<District> districts;
            IQueryable<Region> regions;
            IQueryable<Country> countries;

            model = new OutpostOverviewModel();

            outposts = QueryService.Query();

            if ((countryId == null) && (regionId == null) && (districtId == null))
            {
                model = new OutpostOverviewModel(QueryCountry, QueryRegion, QueryDistrict);
                Guid districtSelectedId = new Guid();

                if (model.Districts.Count > 0)
                {
                    districtSelectedId = Guid.Parse(model.Districts.First().Value);
                }
                if (outposts != null)
                {
                    outposts = outposts.Where(it => it.District.Id == districtSelectedId);
                }
            }
            else
            {
                if ((countryId.Value != Guid.Empty) && (regionId.Value != Guid.Empty) && (districtId.Value != Guid.Empty))
                {
                    var countryS = countryId.Value;
                    countries = QueryCountry.Query().Where(it => it.Id == countryS);
                    regions = QueryRegion.Query().Where<Region>(it => it.Country.Id == countryS);
                    districts = QueryDistrict.Query().Where<District>(it => it.Region.Id == regionId.Value);
                    outposts = outposts.Where<Outpost>(it => it.District.Id == districtId.Value);

                    model = new OutpostOverviewModel();

                    foreach (Country item in countries)
                    {
                        model.Countries.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                    }

                    foreach (Region item in regions)
                    {
                        model.Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

                     }

                    foreach (District itemDistrict in districts)
                    {
                            model.Districts.Add(new SelectListItem { Text = itemDistrict.Name, Value = itemDistrict.Id.ToString() });
                    }

                    if (model.Countries.Count > 0)
                    {
                        var selectedCountry = model.Countries.First<SelectListItem>(it => it.Value == countryId.Value.ToString());
                        if (selectedCountry != null)
                            selectedCountry.Selected = true;
                    }

                    var regionsWithRegionId = model.Regions.Where(it => it.Value == countryId.Value.ToString()).ToList();
                    if (regionsWithRegionId.Count > 0)
                        regionsWithRegionId[0].Selected = true;

                    var regionsWithDistrictsId = model.Districts.Where(it => it.Value == districtId.Value.ToString()).ToList();
                    if (regionsWithDistrictsId.Count > 0)
                        regionsWithDistrictsId[0].Selected = true;
                }
                else
                {
                }
            }


            if (outposts != null)
            {
                if (outposts.ToList().Count() > 0)
                    outposts.ToList().ForEach(item =>
                    {
                        CreateMappings();
                        var outpostModel = new OutpostModel();
                        Mapper.Map(item, outpostModel);
                        model.Outposts.Add(outpostModel);
                    });
            }

            return View(model);

        }

        [HttpGet]
        public PartialViewResult OverviewOutpost(Guid? districtId)
        {
            var outpostsList = new List<OutpostModel>();
            if (!districtId.HasValue)
                return PartialView(outpostsList);

            var outposts = QueryService.Query().Where<Outpost>(it => it.District.Id == districtId);

            foreach (Outpost item in outposts)
            {
                CreateMappings();
                var outpostModel = new OutpostModel();
                Mapper.Map(item, outpostModel);
                //outpostModel.DistrictNo = QueryDistrict.Query().Count<District>(it => it.Region.Id == item.Id);
                outpostsList.Add(outpostModel);

            }
            return PartialView(outpostsList);
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create()
        {
            var model = CreateOutpost;
            //model.Region.CountryId = 
            //model.Region.Id = 
            //model.Region.DistrictId = 
            //model.Country = QueryCountry.Load(outpostInputModel.Region.CountryId);
            //model.Region = QueryRegion.Load(outpostInputModel.Region.Id);
            //model.District = QueryDistrict.Load(outpostInputModel.District.Id);

            return View(model);
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid outpostId)
        {
            Outpost outpost = new Outpost();
            var _outpost = QueryService.Load(outpostId);
            var outpostModelView = new OutpostOutputModel(QueryCountry, QueryRegion, QueryDistrict);
            CreateMappings();
            Mapper.Map(_outpost, outpostModelView);

            var selectedCountry = outpostModelView.Countries.Where<SelectListItem>(it => it.Value == _outpost.District.Region.Country.Id.ToString()).ToList();
            if (selectedCountry.Count > 0)
            {
                selectedCountry[0].Selected = true;
                Guid selectedCountryId = Guid.Parse(selectedCountry[0].Value);
                var regions = QueryRegion.Query().Where(it => it.Country.Id == selectedCountryId);

                if (regions.ToList().Count > 0)
                {
                    foreach (Region item in regions)
                    {
                        outpostModelView.Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                    }
                }
            }

            var selectedRegion = outpostModelView.Regions.Where<SelectListItem>(it => it.Value == _outpost.District.Region.Id.ToString()).ToList();
            if (selectedRegion.Count > 0)
                outpostModelView.Regions.First<SelectListItem>(it => it.Value == _outpost.District.Region.Id.ToString()).Selected = true;

            var selectedDistrict = outpostModelView.Regions.Where<SelectListItem>(it => it.Value == _outpost.District.Id.ToString()).ToList();
            if (selectedDistrict.Count > 0)
                outpostModelView.Districts.First<SelectListItem>(it => it.Value == _outpost.District.Id.ToString()).Selected = true;

            return View(outpostModelView);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(OutpostInputModel outpostInputModel)
        {
            var model = new OutpostInputModel();

            if (!ModelState.IsValid)
            {
                var outpostsOutputModel = new OutpostOutputModel();
                outpostsOutputModel.Region = new RegionModel();
                outpostsOutputModel.District = new DistrictModel();
                outpostsOutputModel.Regions = new List<SelectListItem>();
                outpostsOutputModel.Districts = new List<SelectListItem>();
                outpostsOutputModel = MapDatFromInputModelToOutputModel(outpostInputModel);
                return View("Create", outpostsOutputModel);
            }
            CreateMappings();
            var outpost = new Outpost();
            var client = QueryClients.Load(Client.DEFAULT_ID);
            Mapper.Map(outpostInputModel, outpost);

            outpost.Client = client;

            outpost.Country = QueryCountry.Load(outpostInputModel.Region.CountryId);
            outpost.Region = QueryRegion.Load(outpostInputModel.Region.Id);
            outpost.District = QueryDistrict.Load(outpostInputModel.District.Id);

            SaveOrUpdateCommand.Execute(outpost);
            return RedirectToAction("Overview", "Outpost", new
                    {
                        countryId = outpostInputModel.Region.CountryId,
                        regionId = outpostInputModel.Region.Id,
                        districtId = outpostInputModel.District.Id
                    });
        }


        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(OutpostInputModel outpostInputModel)
        {
            var model = new OutpostInputModel();

            if (!ModelState.IsValid)
            {
                var outpostOutputModel = MapDatFromInputModelToOutputModel(outpostInputModel);
                return View("Edit", outpostOutputModel);
            }

            CreateMappings();
            var _outpost = new Outpost();
            Mapper.Map(outpostInputModel, _outpost);

            _outpost.Country = QueryCountry.Load(outpostInputModel.Region.CountryId);

            if (outpostInputModel.Region != null)
            {
                _outpost.Region = QueryRegion.Load(outpostInputModel.Region.Id);
            }

            if (outpostInputModel.District != null)
            {

                _outpost.District = QueryDistrict.Load(outpostInputModel.District.Id);
            }

            SaveOrUpdateCommand.Execute(_outpost);

            return RedirectToAction("Overview",
                new
                {
                    countryId = outpostInputModel.Region.CountryId,
                    regionId = outpostInputModel.Region.Id,
                    districtId = outpostInputModel.District.Id
                });

        }

        private static void CreateMappings()
        {
            Mapper.CreateMap<OutpostModel, Outpost>();
            Mapper.CreateMap<Outpost, OutpostModel>();

            Mapper.CreateMap<Outpost, OutpostInputModel>();
            Mapper.CreateMap<Outpost, OutpostOutputModel>();
            //.ForMember("Region", m => m.Ignore())
            //.ForMember("District", m => m.Ignore())
            //.ForMember("Country", m=> m.Ignore());


            Mapper.CreateMap<OutpostInputModel, Outpost>()
                    .ForMember("Region", m => m.Ignore())
                    .ForMember("District", m => m.Ignore())
                    .ForMember("Country", m => m.Ignore());

            Mapper.CreateMap<OutpostOutputModel, Outpost>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<Region, RegionModel>();
            Mapper.CreateMap<RegionModel, Region>();

            Mapper.CreateMap<District, DistrictModel>();
            Mapper.CreateMap<DistrictModel, District>();

            Mapper.CreateMap<Client, ClientModel>();
            Mapper.CreateMap<ClientModel, Client>();

        }

        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid outpostId)
        {
            var outpost = QueryService.Load(outpostId);
            //var products = QueryProduct.Query().Where(m => m.OutpostId == outpostId);


            if (outpost != null)
            {
                //if (products.ToList().Count != 0)
                //{
                    //TempData.Add("error", string.Format("The Outpost {0} has products associated, so it can not be deleted", outpost.Name));
                    //return RedirectToAction("Overview", "Outpost", new
                    //{
                    //    countryId = outpost.Country.Id,
                    //    regionId = outpost.Region.Id,
                    //    districtId = outpost.Region.Id
                    //});
                //}
                DeleteCommand.Execute(outpost);
            }

            return RedirectToAction("Overview", "Outpost", new
            {
                countryId = outpost.District.Region.Country.Id,
                regionId = outpost.District.Region.Id,
                districtId = outpost.District.Id
            });
        }

        [HttpGet]
        public JsonResult GetDistrictsForRegion(Guid? regionId)
        {
            List<SelectListItem> Districts = new List<SelectListItem>();

            var districts = QueryDistrict.Query().Where(it => it.Region.Id == regionId.Value);

            if (districts.ToList().Count > 0)
            {
                foreach (var item in districts)
                {
                    Districts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Districts;

            return Json(Districts, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetOutpostsForDistrict(Guid? districtId)
        {
            List<SelectListItem> Outposts = new List<SelectListItem>();

            var outposts = QueryService.Query().Where(it => it.District.Id == districtId.Value);

            if (outposts.ToList().Count > 0)
            {
                foreach (var item in outposts)
                {
                    Outposts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Outposts;

            return Json(Outposts, JsonRequestBehavior.AllowGet);

        }

        private OutpostOutputModel MapDatFromInputModelToOutputModel(OutpostInputModel outpostInputModel)
        {
            var outpostOutputModel = new OutpostOutputModel(QueryCountry, QueryRegion, QueryDistrict);
            var countries = QueryCountry.Query().Where(it => it.Id == outpostInputModel.Region.CountryId);

            //var districts = QueryDistrict.Query().Where(it1 => it1.Id = outpostInputModel.District.Id);
            //var outposts = QueryOutpost.Query().Where(it2 => it2.District.Id = outpostInputModel.District.Id);

            if (countries.ToList().Count > 0)
            {
                foreach (Country item in countries)
                {
                    outpostOutputModel.Countries.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
                }
            }
            var selectedCountry = outpostOutputModel.Countries.Where(it => it.Value == outpostInputModel.Region.CountryId.ToString()).ToList();
            if (selectedCountry.Count > 0)
                outpostOutputModel.Countries.First(it => it.Value == outpostInputModel.Region.CountryId.ToString()).Selected = true;

            if (QueryRegion.Query() != null)
            {
                var regions = QueryRegion.Query().Where(it => it.Country.Id == outpostInputModel.Region.CountryId);
                if (regions.ToList().Count > 0)
                {
                    foreach (Region item in regions)
                    {
                        outpostOutputModel.Regions.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
                    }
                }
                var selectedRegion = outpostOutputModel.Regions.Where(it => it.Value == outpostInputModel.Region.Id.ToString()).ToList();
                var selectedDistrict = outpostOutputModel.Districts.Where(it => it.Value == outpostInputModel.District.Id.ToString()).ToList();


                if (selectedRegion.Count > 0)
                    outpostOutputModel.Regions.First(it => it.Value == outpostInputModel.Region.Id.ToString()).Selected = true;

                if (selectedDistrict.Count > 0)
                    outpostOutputModel.Districts.First(it => it.Value == outpostInputModel.District.Id.ToString()).Selected = true;
            }

            outpostOutputModel.Client = new ClientModel
            {
                Id = Client.DEFAULT_ID
            };


            outpostOutputModel.Region = new RegionModel();
            outpostOutputModel.District = new DistrictModel();
            outpostOutputModel.Id = outpostInputModel.Id;
            outpostOutputModel.Name = outpostInputModel.Name;
            outpostOutputModel.Region.CountryId = outpostInputModel.Region.CountryId;
            outpostOutputModel.Region.Id = outpostInputModel.Region.Id;
            outpostOutputModel.District.Id = outpostInputModel.District.Id;


            return outpostOutputModel;
        }
    }
}
