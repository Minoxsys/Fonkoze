﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Validation.ValidDate;
using System.Globalization;
using Persistence.Queries.Districts;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Client;
using Core.Domain;
using Web.Models.Shared;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class DistrictController : Controller
    {
        public ISaveOrUpdateCommand<District> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<District> DeleteCommand { get; set; }

        public IQueryService<Region> QueryRegion { get; set; }

        public IQueryService<Outpost> QueryOutpost { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Country> QueryCountry { get; set; }

        public IQueryDistrict QueryDistrict { get; set; }

        public IQueryService<District> QueryService { get; set; }
        public IQueryService<Client> LoadClient { get; set; }

        public IQueryService<User> QueryUsers { get; set; }

        private const string MESSAGE_ERROR_ON_SUCCESS = "done";
        private Client _client;
        private User _user;

        public ActionResult Overview()
        {
            return View();
        }
        public JsonResult Index(DistrictIndexModel indexModel)
        {
            var districts = QueryService.Query().Take(0);
            LoadUserAndClient();
            int pageSize =0;

            if ((indexModel.Limit != null) && (indexModel.Start != null))
                pageSize = indexModel.Limit.Value - indexModel.Start.Value;

            if ((indexModel.RegionId == null) && (indexModel.CountryId == null))
            {
                if (indexModel.SearchName != null)
                {
                    districts = QueryService.Query().Where(it => it.Client == _client && it.Name.Contains(indexModel.SearchName));
                      
                }
            }
            else
            {
                if (indexModel.CountryId != null)
                {
                    if (indexModel.SearchName != null)
                    {
                        districts = QueryService.Query().Where(it => it.Region.Country.Id == indexModel.CountryId.Value && it.Client == _client && it.Name.Contains(indexModel.SearchName));
                           
                    }
                    else
                    {
                        districts = QueryService.Query().Where(it => it.Region.Country.Id == indexModel.CountryId.Value && it.Client == _client);
                            

                    }
                }
                else
                {
                    if (indexModel.SearchName == null)
                    {
                        districts = QueryService.Query().Where(it => it.Region.Id == indexModel.RegionId.Value && it.Client == _client);
                            
                    }
                    else
                    {
                        districts = QueryService.Query().Where(it => it.Region.Id == indexModel.RegionId.Value && it.Client == _client && it.Name.Contains(indexModel.SearchName));
                            
                    }
                }
            }

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<District>>>()
            {
                { "Name-ASC", () => districts.OrderBy(it=>it.Name) },
                { "Name-DESC", () => districts.OrderByDescending(c => c.Name) },               
            };
            if (indexModel.sort != "OutpostNo")
            {
                districts = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

                districts = districts.Take(pageSize)
                                                   .Skip(indexModel.Start.Value);
            }

            var districtModelList = new List<DistrictModel>();

            
            foreach (var district in districts.ToList())
            {
                var districtModel = new DistrictModel();
                districtModel.Name = district.Name;
                districtModel.Id = district.Id;
                districtModel.ClientId = district.Client.Id;
                districtModel.RegionId = district.Region.Id;
                districtModel.OutpostNo = QueryOutpost.Query().Count(it => it.District.Id == district.Id && it.Client == _client);
                districtModelList.Add(districtModel);

            }
            if (indexModel.sort.Equals("OutpostNo"))
            {
                if (indexModel.dir.Equals("DESC"))
                {
                    districtModelList = districtModelList.OrderByDescending(it => it.OutpostNo).ToList();
                }
                else
                {
                    districtModelList = districtModelList.OrderBy(it => it.OutpostNo).ToList();
 
                }
                districtModelList = districtModelList.Take(pageSize).Skip(indexModel.Start.Value).ToList();
            }

            return Json(new
            {
                districts = districtModelList,
                TotalItems = districtModelList.Count
            }, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetCountries()
        {
            LoadUserAndClient();

            var countries = QueryCountry.Query().Where(it => it.Client == _client).ToList();
            var countryModelList = new List<CountryModel>();

            foreach (var country in countries)
            {
                var countryModel = new CountryModel();
                countryModel.Id = country.Id;
                countryModel.Name = country.Name;
                countryModelList.Add(countryModel);

            }

            return Json(new
            {
                countries = countryModelList
            ,
                TotalItems = countryModelList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegions(Guid? countryId)
        {
            LoadUserAndClient();

            var regionModelList = new List<RegionModel>();
            var regions = new List<Region>();

            if (countryId != null)
            {
                regions = QueryRegion.Query().Where(it => it.Country.Id == countryId.Value && it.Client == _client).ToList();

                foreach (var region in regions)
                {
                    var regionModel = new RegionModel();
                    regionModel.Name = region.Name;
                    regionModel.Id = region.Id;
                    regionModelList.Add(regionModel);
                }

            }
            return Json(new
            {
                regions = regionModelList,
                TotalItems = regionModelList.Count
            }, JsonRequestBehavior.AllowGet);


        }

        private void CreateMapping()
        {
            Mapper.CreateMap<DistrictModel, District>();
            Mapper.CreateMap<District, DistrictModel>();

            Mapper.CreateMap<DistrictInputModel, District>().ForMember("Region",
                m => m.Ignore());
            Mapper.CreateMap<DistrictOutputModel, District>();

            Mapper.CreateMap<ClientModel, Client>();
            Mapper.CreateMap<RegionModel, Region>();

            Mapper.CreateMap<Region, RegionModel>();
            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();
            Mapper.CreateMap<Client, ClientModel>();

            Mapper.CreateMap<DistrictInputModel.ClientInputModel, Client>();
            Mapper.CreateMap<DistrictInputModel.RegionInputModel, Region>();
            Mapper.CreateMap<District, DistrictOutputModel>();
            Mapper.CreateMap<District, DistrictInputModel>();
        }

        [HttpPost]
        public ActionResult Create(DistrictInputModel districtInputModel)
        {
            LoadUserAndClient();

            if (!ModelState.IsValid)
            {
                return Json(
                    new JsonActionResponse
                    {
                        Status = "Error",
                        Message = "The district has not been saved!"
                    });
               
            }

            CreateMapping();
            var district = new District();
            Mapper.Map(districtInputModel, district);

            var client = QueryClients.Load(_client.Id);
            var region = QueryRegion.Load(districtInputModel.Region.Id);

            district.Client = client;
            district.Region = region;

            SaveOrUpdateCommand.Execute(district);
           
            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("District {0} has been saved.", district.Name)
               });
           
        }

        [HttpPost]
        public ActionResult Edit(DistrictInputModel districtInputModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The district has not been updated!"
                   });
            }

            District district = QueryService.Load(districtInputModel.Id);

            CreateMapping();
            Mapper.Map(districtInputModel, district);

            var region = QueryRegion.Load(districtInputModel.Region.Id);
            var client = QueryClients.Load(districtInputModel.Client.Id);
            district.Region = region;
            district.Client = client;

            SaveOrUpdateCommand.Execute(district);
            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("District {0} has been saved.", district.Name)
                });
       }

        [HttpPost]
        public ActionResult Delete(Guid guid)
        {
            var district = QueryService.Load(guid);
            var jsonActionResponse = new JsonActionResponse();
            jsonActionResponse.Message = MESSAGE_ERROR_ON_SUCCESS;
            jsonActionResponse.Status = "Done";

            if (district != null)
            {
                var districtResults = QueryOutpost.Query().Where(it => it.District.Id == district.Id);

                if (districtResults.ToList().Count != 0)
                {
                    jsonActionResponse.Message = string.Format("The district {0} has outposts associated, so it can not be deleted!", district.Name);
                    jsonActionResponse.Status = "Fail";
                    return Json(new { response = jsonActionResponse }, JsonRequestBehavior.AllowGet);
                }

                DeleteCommand.Execute(district);
            }

            return Json(new { response = jsonActionResponse }, JsonRequestBehavior.AllowGet);
        }
    }
}
