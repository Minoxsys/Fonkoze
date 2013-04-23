using AutoMapper;
using Core.Domain;
using Core.Persistence;
using Core.Security;
using Domain;
using Persistence.Queries.Districts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Client;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Models.Shared;
using Web.Security;

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
        public IQueryService<Role> RolesQueryService { get; set; }

        public IQueryService<User> QueryUsers { get; set; }
        public IPermissionsService PermissionService { get; set; }

        private const String DISTRICT_ADD_PERMISSION = "District.Edit";
        private const String DISTRICT_DELETE_PERMISSION = "District.Delete";

        private Client _client;
        private User _user;

        [HttpGet]
        [Requires(Permissions = "District.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(DISTRICT_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(DISTRICT_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           

            Guid? regionId = (Guid?)TempData["FromRegionsId"];

            FromRegionModel model = new FromRegionModel();
            if (regionId.HasValue)
            {
                var region = QueryRegion.Load(regionId.Value);
                model.RegionId = region.Id;
                model.CountryId = region.Country.Id;
            }

            return View("Overview", model);
        }

        [HttpGet]
        public ActionResult FromRegions(Guid? regionId)
        {
            if (regionId.HasValue)
            {
                TempData.Clear();
                TempData.Add("FromRegionsId", regionId.Value);
            }

            return RedirectToAction("Overview");
        }

        public JsonResult Index(DistrictIndexModel indexModel)
        {
            LoadUserAndClient();

            var districts = QueryService.Query().Where(it => it.Client.Id == _client.Id);

            int pageSize = 0;

            if (indexModel.Limit != null)
                pageSize = indexModel.Limit.Value;

            if (indexModel.CountryId != null && indexModel.CountryId.Value != Guid.Empty)
                districts = districts.Where(it => it.Region.Country.Id == indexModel.CountryId.Value);

            if (indexModel.RegionId != null && indexModel.RegionId.Value != Guid.Empty)
                districts =  districts.Where(it => it.Region.Id == indexModel.RegionId); 

            if (indexModel.SearchName != null)
                districts = districts.Where(it => it.Name.Contains(indexModel.SearchName));


            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<District>>>()
            {
                { "Name-ASC", () => districts.OrderBy(it=>it.Name) },
                { "Name-DESC", () => districts.OrderByDescending(c => c.Name) }               
            };
            int totalItems = 0;

            if (indexModel.sort != "OutpostNo")
            {
                districts = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
                totalItems = districts.Count();
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
                districtModel.CountryId = district.Region.Country.Id;
                districtModel.OutpostNo = QueryOutpost.Query().Count(it => it.District.Id == district.Id && it.Client == _client);
                if (district.DistrictManager != null)
                    districtModel.ManagerId = district.DistrictManager.Id;
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

            return Json(new DistrictIndexOutputModel
            {
                districts = districtModelList,
                TotalItems = totalItems
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
            CountryModel allModel = new CountryModel { Id = Guid.Empty, Name = " All"};
            countryModelList.Add(allModel);

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

            var regions = QueryRegion.Query().Where(it => it.Client.Id == _client.Id);

            var regionModelList = new List<RegionModel>();
            RegionModel allModel = new RegionModel { Id = Guid.Empty, Name = " All"};
            regionModelList.Add(allModel);

            if (countryId != null)
            {
                regions = regions.Where(it => it.Country.Id == countryId.Value);

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
                regions = regionModelList.ToArray(),
                TotalItems = regionModelList.Count
            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetManagers(UserAndClientIdentity loggedInUser)
        {
            Guid managerRoleId = GetManagerRole();

            var managerUsers = QueryUsers.Query().Where(it => it.ClientId == loggedInUser.Client.Id && it.RoleId == managerRoleId);
            var viewModelList =
                managerUsers.Select(managerUser => new ManagerModel
                    {
                        Id = managerUser.Id,
                        Name = managerUser.FirstName + " " + managerUser.LastName
                    }).ToList();

            return Json(new
                {
                    regions = viewModelList.ToArray(),
                    TotalItems = viewModelList.Count()
                }, JsonRequestBehavior.AllowGet);
        }

        private Guid GetManagerRole()
        {
            var managerRole = RolesQueryService.Query().FirstOrDefault(r => r.Name == "Manager");
            if (managerRole != null)
            {
                return managerRole.Id;
            }
            return Guid.Empty;
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
        public JsonResult Create(DistrictInputModel districtInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(districtInputModel.Name))
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

            var districts = QueryService.Query().Where(it =>it.Client.Id == _client.Id && it.Region.Id == districtInputModel.Region.Id && it.Name == districtInputModel.Name);

            if (districts.Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        Message = string.Format("The region already contains a district with the name {0}! Please insert a different name!", districtInputModel.Name),
                        CloseModal = false
                    });
 
            }

            if (districtInputModel.ManagerId != Guid.Empty)
            {
                district.DistrictManager = QueryUsers.Query().FirstOrDefault(u => u.Id == districtInputModel.ManagerId);
            }

            SaveOrUpdateCommand.Execute(district);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("District {0} has been saved.", district.Name)
               });

        }

        [HttpPost]
        public JsonResult Edit(DistrictInputModel districtInputModel)
        {
            LoadUserAndClient();
            if (string.IsNullOrEmpty(districtInputModel.Name))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The district has not been updated!"
                   });
            }

            var districts = QueryService.Query().Where(it => it.Client.Id == _client.Id && it.Id != districtInputModel.Id && it.Region.Id == districtInputModel.Region.Id && it.Name == districtInputModel.Name);

            if (districts.Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        Message = string.Format("The region already contains a district with the name {0}! Please insert a different name!", districtInputModel.Name),
                        CloseModal = false
                    });

            }

            District district = QueryService.Load(districtInputModel.Id);

            CreateMapping();
            Mapper.Map(districtInputModel, district);

            var region = QueryRegion.Load(districtInputModel.Region.Id);
            var client = QueryClients.Load(districtInputModel.Client.Id);
            district.Region = region;
            district.Client = client;

            if (districtInputModel.ManagerId != Guid.Empty)
            {
                district.DistrictManager = QueryUsers.Query().FirstOrDefault(u => u.Id == districtInputModel.ManagerId);
            }

            SaveOrUpdateCommand.Execute(district);
            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("District {0} has been saved.", district.Name)
                });
        }

        [HttpPost]
        public JsonResult Delete(Guid? guid)
        {


            if (guid.HasValue == false)
            {

                return Json(new JsonActionResponse
                {
                    Status = "Error",
                    Message = "You must supply a district id in order to delete the district!"
                }, JsonRequestBehavior.AllowGet);
            }

            var district = QueryService.Load(guid.Value);

            if (district != null)
            {
                var districtResults = QueryOutpost.Query().Where(it => it.District.Id == district.Id);

                if (districtResults.ToList().Count != 0)
                {
                    return Json(new JsonActionResponse 
                    {   Status = "Error",
                        Message = string.Format("The district {0} has outposts associated, so it can not be deleted!", district.Name) 
                    }, JsonRequestBehavior.AllowGet);
                }

                DeleteCommand.Execute(district);


            }
            return Json(new JsonActionResponse 
            {   Status = "Success",
                Message = string.Format("The district {0} has been deleted!", district.Name)
            }, JsonRequestBehavior.AllowGet);



        }
    }
}
