using Core.Domain;
using Core.Persistence;
using Core.Security;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using Web.Models.Shared;
using Web.Security;

namespace Web.Areas.StockAdministration.Controllers
{
    public class OutpostStockLevelController : Controller
    {
        public IQueryService<Country> QueryCountry { get; set; }

        public IQueryService<Region> QueryRegion { get; set; }

        public IQueryService<District> QueryDistrict { get; set; }

        public IQueryService<Outpost> QueryOutpost { get; set; }

        public IQueryService<Product> QueryProduct { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }

        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> SaveOrUpdateOutpostStockLevelHistorical { get; set; }

        public IQueryService<OutpostHistoricalStockLevel> QueryOutpostStockLevelHistorical { get; set; }

        public ISaveOrUpdateCommand<OutpostStockLevel> SaveOrUpdateOutpostStockLevel { get; set; }

        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IPermissionsService PermissionService { get; set; }

        private const String CurrentoutpoststocklevelEditPermission = "CurrentOutpostStockLevel.Edit";

        private Client _client;
        private User _user;

        Guid GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = Guid.Parse("00000000-0000-0000-0000-000000000000");

        [Requires(Permissions = "CurrentOutpostStockLevel.View")]
        public ActionResult Overview(Guid? outpostId)
        {
            ViewBag.HasNoRightsToEdit = PermissionService.HasPermissionAssigned(CurrentoutpoststocklevelEditPermission, User.Identity.Name) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            OutpostOverviewModel model = new OutpostOverviewModel();
            if (outpostId.HasValue && outpostId.Value != Guid.Empty)
            {
                var outpost = QueryOutpost.Load(outpostId.Value);
                model.DistrictId = outpost.District.Id;
                model.RegionId = outpost.Region.Id;
                model.CountryId = outpost.Country.Id;
                model.OutpostId = outpost.Id;
            }

            return View(model);
        }

      

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            _user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            _client = QueryClients.Load(clientId);
        }

        public JsonResult GetCountries()
        {
            LoadUserAndClient();

            var countries = QueryCountry.Query().Where(it => it.Client.Id == _client.Id);
            var countriesList = new List<EntityModel>();
            var allModel = new EntityModel { Id = Guid.Empty, Name = " All" };
            countriesList.Add(allModel);

            foreach (var country in countries)
            {
                var model = new EntityModel();
                model.Name = country.Name;
                model.Id = country.Id;
                countriesList.Add(model);
            }

            return Json(new
            {
                countries = countriesList,
                TotalItems = countriesList.Count
            }, JsonRequestBehavior.AllowGet); 
        }

        public JsonResult GetOutpostStockLevelData(OverviewInputModel input)
        {
            var outpostStockLevelCurrentTreeModel = new OutpostStockLevelCurrentTreeModel { Name = "root" };

            LoadUserAndClient();

            var queryOutpost = QueryOutpost.Query().Where(it => it.Client.Id == _client.Id);
            if (input.CountryId != Guid.Empty)
                queryOutpost = queryOutpost.Where(it => it.Country.Id == input.CountryId);
            if (input.RegionId != Guid.Empty)
                queryOutpost = queryOutpost.Where(it => it.Region.Id == input.RegionId);
            if (input.DistrictId != Guid.Empty)
                queryOutpost = queryOutpost.Where(it => it.District.Id == input.DistrictId);

            if (input.OutpostId.Equals(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST))
            {
                foreach (var outpost in queryOutpost.ToList())
                {
                    var treeModel = ToOutpostNode(input, outpost);
                    outpostStockLevelCurrentTreeModel.children.Add(treeModel);                   
                }
            }
            else
            {
                var outpost =  QueryOutpost.Load(input.OutpostId);

                var treeModel = ToOutpostNode(input, outpost);
                outpostStockLevelCurrentTreeModel.children.Add(treeModel);                   
            }

            return Json(outpostStockLevelCurrentTreeModel, JsonRequestBehavior.AllowGet);
        }

        private OutpostStockLevelCurrentTreeModel ToOutpostNode(OverviewInputModel input, Outpost outpost)
        {
            var treeModel = new OutpostStockLevelCurrentTreeModel
                {
                    Name = outpost.Name,
                    ProductLevel = -1
                };

            var outpostStockLevelCurrentGroupedByProductGroup = GetOutpostStockLevelsGroupedByProductGroup(outpost.Id);

            foreach (var outpostStockLevelGrouping in outpostStockLevelCurrentGroupedByProductGroup)
            {
                var isExpanded = (input.ProductGroupExpandedOnEdit != null) && (input.ProductGroupExpandedOnEdit.Equals(outpostStockLevelGrouping.Key.Name));
                var productGroupTreeModel = ToProductGroupNode(outpostStockLevelGrouping);

                productGroupTreeModel.expanded = isExpanded;
                treeModel.expanded = isExpanded;

                treeModel.children.Add(productGroupTreeModel);
            }
            return treeModel;
        }

        private static OutpostStockLevelCurrentTreeModel ToProductGroupNode(IGrouping<ProductGroup, OutpostStockLevel> outpostStockLevelGrouping)
        {
            var productGroupTreeModel = new OutpostStockLevelCurrentTreeModel
                {
                    Name = outpostStockLevelGrouping.Key.Name,
                    ProductLevel = -1
                };

            foreach (var outpostStockLevel in outpostStockLevelGrouping)
            {
                var productTreeModel = ToProductLeafNode(outpostStockLevel);
                productGroupTreeModel.children.Add(productTreeModel);
            }

            return productGroupTreeModel;
        }

        private static OutpostStockLevelCurrentTreeModel ToProductLeafNode( OutpostStockLevel outpostStockLevel)
        {
            var productTreeModel = new OutpostStockLevelCurrentTreeModel();

            var productEntity = outpostStockLevel.Product;
           
            productTreeModel.Name = productEntity.Name;
            productTreeModel.Description = productEntity.Description;
            productTreeModel.SMSCode = productEntity.SMSReferenceCode;

            productTreeModel.Id = outpostStockLevel.Id;

            productTreeModel.PreviousLevel = outpostStockLevel.PrevStockLevel;
            productTreeModel.ProductLevel = outpostStockLevel.StockLevel;

            if (outpostStockLevel.Updated != null) 
                productTreeModel.LastUpdate = outpostStockLevel.Updated.Value.ToString("dd-MMM-yyyy");

            productTreeModel.UpdateMethod = outpostStockLevel.UpdateMethod;
            
            productTreeModel.ProductGroupName = outpostStockLevel.ProductGroup.Name;
            
            productTreeModel.OutpostName = outpostStockLevel.Outpost.Name;

            productTreeModel.leaf = true;

            return productTreeModel;
        }

        private IEnumerable<IGrouping<ProductGroup, OutpostStockLevel>> GetOutpostStockLevelsGroupedByProductGroup(Guid outpostId)
        {
            var outpostStockLevelCurrentGroupedByProductGroup = QueryOutpostStockLevel
                                                                                      .Query()
                                                                                      .Where(it => it.Client.Id == _client.Id)
                                                                                      .Where(it => it.Outpost.Id == outpostId)
                                                                                      .ToList()
                                                                                      .OrderBy(it => it.Updated)
                                                                                      .GroupBy(it => it.ProductGroup);
            return outpostStockLevelCurrentGroupedByProductGroup;
        }

        [HttpPost]
        public JsonResult Edit(OutpostStockLevelInputModel outpostStockLevelInput)
        {
            LoadUserAndClient();

            if (outpostStockLevelInput.Id != null)
            {
                var outpostStockLevel = QueryOutpostStockLevel.Load(outpostStockLevelInput.Id.Value);

                var outpostHistoricalStockLevel = SetOutpostHistoricalStockLevelFromPreviousOutpostStockLevelCurrentData(outpostStockLevel);

                outpostStockLevel.PrevStockLevel = outpostStockLevel.StockLevel;
                outpostStockLevel.StockLevel = outpostStockLevelInput.StockLevel;
                outpostStockLevel.UpdateMethod = OutpostStockLevel.MANUAL_UPDATE;

                SaveOrUpdateOutpostStockLevelHistorical.Execute(outpostHistoricalStockLevel);
                SaveOrUpdateOutpostStockLevel.Execute(outpostStockLevel);
            }

            return Json(new JsonActionResponse { Status = "Success", Message = "Outpost Stock Level successfully updated !" }, JsonRequestBehavior.AllowGet);
        }

        public OutpostHistoricalStockLevel SetOutpostHistoricalStockLevelFromPreviousOutpostStockLevelCurrentData(OutpostStockLevel outpostStockLevel)
        {
            var outpostHistoricalStockLevel = new OutpostHistoricalStockLevel();

            outpostHistoricalStockLevel.ByUser = _user;
            outpostHistoricalStockLevel.ClientId = _client.Id;


            outpostHistoricalStockLevel.OutpostId = outpostStockLevel.Outpost.Id;
            outpostHistoricalStockLevel.OutpostName = outpostStockLevel.Outpost.Name;

            outpostHistoricalStockLevel.PrevStockLevel = outpostStockLevel.PrevStockLevel;

            outpostHistoricalStockLevel.ProductGroupId = outpostStockLevel.ProductGroup.Id;
            outpostHistoricalStockLevel.ProductGroupName = outpostStockLevel.ProductGroup.Name;

            outpostHistoricalStockLevel.ProdSmsRef = outpostStockLevel.Product.SMSReferenceCode;
            outpostHistoricalStockLevel.ProductId = outpostStockLevel.Product.Id;
            outpostHistoricalStockLevel.ProductName = outpostStockLevel.Product.Name;

            outpostHistoricalStockLevel.StockLevel = outpostStockLevel.StockLevel;
            outpostHistoricalStockLevel.UpdateDate = outpostStockLevel.Updated;
            outpostHistoricalStockLevel.UpdateMethod = outpostStockLevel.UpdateMethod;

            return outpostHistoricalStockLevel;
        }

        public JsonResult GetRegions(Guid? countryId)
        {
            LoadUserAndClient();

            var regions = new List<Region>();
            var regionList = new List<EntityModel>();
            var allModel = new EntityModel { Id = Guid.Empty, Name = " All"};
            regionList.Add(allModel);

            if (countryId != null && countryId.Value != Guid.Empty)
            {
                regions = QueryRegion.Query().Where(it => it.Country.Id == countryId.Value && it.Client.Id == _client.Id).ToList();
            }
            foreach (var region in regions)
            {
                var model = new EntityModel();
                model.Name = region.Name;
                model.Id = region.Id;
                regionList.Add(model);
            }

            return Json(new
            {
                regions = regionList,
                TotalItems = regionList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistricts(Guid? regionId)
        {
            LoadUserAndClient();

            var districts = new List<District>();
            var districtList = new List<EntityModel>();
            var allModel = new EntityModel { Id = Guid.Empty, Name = " All" };
            districtList.Add(allModel);

            if (regionId != null && regionId.Value != Guid.Empty)
            {
                districts = QueryDistrict.Query().Where(it => it.Region.Id == regionId.Value && it.Client.Id == _client.Id).ToList();
            }
            foreach (var district in districts)
            {
                var model = new EntityModel();
                model.Name = district.Name;
                model.Id = district.Id;
                districtList.Add(model);
            }

            return Json(new
            {
                districts = districtList,
                TotalItems = districtList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOutposts(Guid? districtId)
        {
            LoadUserAndClient();

            var outposts = new List<Outpost>();
            var outpostList = new List<EntityModel>();

            var modelForAllOption = new EntityModel();
            modelForAllOption.Id = GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST;
            modelForAllOption.Name = "All";
            outpostList.Add(modelForAllOption);

            if (districtId != null && districtId.Value != Guid.Empty)
            {
                outposts = QueryOutpost.Query().Where(it => it.District.Id == districtId.Value && it.Client.Id == _client.Id).ToList();
            }
            foreach (var outpost in outposts)
            {
                var model = new EntityModel();
                model.Name = outpost.Name;
                model.Id = outpost.Id;
                outpostList.Add(model);
            }

            return Json(new
            {
                outposts = outpostList,
                TotalItems = outpostList.Count
            }, JsonRequestBehavior.AllowGet);
        }
    }
}