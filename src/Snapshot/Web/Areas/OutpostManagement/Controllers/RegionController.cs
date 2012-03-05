using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Region;
using AutoMapper;
using Web.Areas.OutpostManagement.Models.Country;
using Persistence.Queries.Countries;
using Web.Areas.OutpostManagement.Models.Client;
using System.Collections;
using Core.Domain;
using Web.Models.Shared;
using NHibernate.Linq;
using Web.Areas.StockAdministration.Models.Product;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class RegionController : Controller
    {
        public IQueryService<Region> QueryService { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public ISaveOrUpdateCommand<Region> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Region> DeleteCommand { get; set; }

        private Client _client;
        private User _user;

        [HttpGet]
        public ActionResult Overview()
        {
            Guid? countryId = (Guid?)TempData["CountryId"];

            FromCountryModel model = new FromCountryModel();
            if (countryId.HasValue)
                model.CountryId = countryId.Value;

            return View("Overview", model);
        }

        [HttpGet]
        public ActionResult FromCountries(Guid? countryId)
        {
            if (countryId.HasValue)
                TempData.Add("CountryId", countryId.Value);

            return RedirectToAction("Overview", "Region");
        }

        [HttpPost]
        public JsonResult Create(RegionInputModel regionInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(regionInputModel.Name) || string.IsNullOrEmpty(regionInputModel.CountryId.ToString()))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The region has not been saved!"
                   });
            }

            var queryRegionValidation = QueryService.Query().Where(p=>p.Client == _client);
            if (queryRegionValidation.Where(it => it.Name == regionInputModel.Name && it.Country.Id == regionInputModel.CountryId).Count()> 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                   Status = "Error",
                   CloseModal = false,
                   Message = string.Format("There is already associated a region with the name {0} for this country! Please insert a different name!",regionInputModel.Name)
                    });
 
            }

            if (queryRegionValidation.Where(it => it.Coordinates == regionInputModel.Coordinates).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a region with coordinates {0}! Please insert different coordinates!", regionInputModel.Coordinates)
                    });
            }
            Region region = new Region();

            CreateMapping();
            Mapper.Map(regionInputModel, region);

            region.Client = this._client;
            region.Country = QueryCountry.Load(regionInputModel.CountryId);

            SaveOrUpdateCommand.Execute(region);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Region {0} has been saved.", region.Name)
               });
        }

        [HttpPost]
        public JsonResult Edit(RegionInputModel regionInputModel)
        {
            LoadUserAndClient();

            if (regionInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a regionId in order to edit the region."
                   });
            }

            if (string.IsNullOrEmpty(regionInputModel.Name) || string.IsNullOrEmpty(regionInputModel.CountryId.ToString()))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The region has not been saved!"
                   });
            }

            var queryRegionValidation = QueryService.Query().Where(p=>p.Client == _client);

            if (queryRegionValidation.Where(it => it.Name == regionInputModel.Name && it.Id != regionInputModel.Id && it.Country.Id == regionInputModel.CountryId).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a region with the name {0} for this country! Please insert a different name!", regionInputModel.Name)
                    });

            }

            if (queryRegionValidation.Where(it => it.Coordinates == regionInputModel.Coordinates && it.Id != regionInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a region with coordinates {0}! Please insert different coordinates!", regionInputModel.Coordinates)
                    });
            }

            Region region = new Region();

            CreateMapping();
            Mapper.Map(regionInputModel, region);

            region.Country = QueryCountry.Load(regionInputModel.CountryId);
            region.ByUser = this._user;
            region.Client = this._client;

            SaveOrUpdateCommand.Execute(region);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal =true,
                   Message = String.Format("Region {0} has been saved.", region.Name)
               });
        }

        [HttpPost]
        public JsonResult Delete(Guid? regionId)
        {
            LoadUserAndClient();

            if (regionId.HasValue == false)
            {
                return Json(new JsonActionResponse
                {
                    Status = "Error",
                    Message = "You must supply a regionId in order to remove the region"
                });
            }

            var region = QueryService.Load(regionId.Value);
            if (region != null)
            {
                var districtResults = QueryDistrict.Query().Where(it => it.Region.Id == region.Id);
                if (districtResults.ToList().Count != 0)
                {
                    return Json(new JsonActionResponse
                    {
                        Status = "Error",
                        Message = String.Format("Region {0} has {1} district(s) associated, and can not be removed.", region.Name, districtResults.ToList().Count)
                    });
                }
                DeleteCommand.Execute(region);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Region {0} was removed.", region.Name)
                });
        }

        [HttpGet]
        public JsonResult GetRegions(RegionIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var regionDataQuery = this.QueryService.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Region>>>()
            {
                { "Name-ASC", () => regionDataQuery.OrderBy(c => c.Name) },
                { "Name-DESC", () => regionDataQuery.OrderByDescending(c => c.Name) },
                { "Coordinates-ASC", () => regionDataQuery.OrderBy(c => c.Coordinates) },
                { "Coordinates-DESC", () => regionDataQuery.OrderByDescending(c => c.Coordinates) },
            };

            regionDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.countryId))
            {
                Guid id = new Guid(indexModel.countryId);
                regionDataQuery = regionDataQuery
                    .Where(it => it.Country.Id == id);
            }
                regionDataQuery = regionDataQuery
                    .Where(it => it.Client.Id == this._client.Id);

            var totalItems = regionDataQuery.Count();

            regionDataQuery = regionDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);
            
            var regionModelListProjection = (from region in regionDataQuery.ToList()
                                             select new RegionModel
                                             {
                                                 Id = region.Id,
                                                 Name = region.Name,
                                                 Coordinates = region.Coordinates,
                                                 CountryId = region.Country.Id,
                                             }).ToArray();


            return Json(new RegionIndexOuputModel
            {
                Regions = regionModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
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

            this._client = QueryClients.Load(clientId);
        }


        [HttpGet]
        public JsonResult GetCountries()
        {
            LoadUserAndClient();
            var countries = QueryCountry.Query().Where(it => it.Client.Id == this._client.Id);
            int totalItems = countries.Count();

            var countryModelListProjection = (from country in countries.ToList()
                                              select new CountryModel
                                              {
                                                  Id = country.Id,
                                                  Name = country.Name,
                                              }).ToArray();


            return Json(new CountryIndexOutputModel
            {
                Countries = countryModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private void CreateMapping()
        {
            Mapper.CreateMap<RegionModel, Region>();
            Mapper.CreateMap<Region, RegionModel>();

            Mapper.CreateMap<Region, RegionInputModel>();
            Mapper.CreateMap<Region, RegionOutputModel>();

            Mapper.CreateMap<RegionInputModel, Region>();
            Mapper.CreateMap<RegionOutputModel, Region>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<ClientModel, Client>();
            Mapper.CreateMap<Client, ClientModel>();

        }
    }

}
