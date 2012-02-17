using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using Domain;
using Core.Persistence;
using Web.Areas.StockAdministration.Models.Product;
using AutoMapper;
using Core.Domain;
using System.Web.Script.Serialization;
using Web.Models.Shared;

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

        private const string UPDATE_METHOD_SYSTEM = "System";
        private Client _client;
        private User _user;

        Guid GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public ActionResult Overview()
        {
            return View();
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }

        public JsonResult GetCountries()
        {
            LoadUserAndClient();

            var countries = QueryCountry.Query().Where(it=>it.Client.Id == _client.Id);
            var countriesList = new List<EntityModel>();

            foreach (var country in countries)
            {
                var model = new EntityModel();
                model.Name = country.Name;
                model.Id = country.Id;
                countriesList.Add(model);

            }

            return Json(new
            {
                countries = countriesList
            ,
                TotalItems = countriesList.Count
            }, JsonRequestBehavior.AllowGet); 
        }
        public JsonResult GetOutpostStockLevelData(OverviewInputModel input)
        {
            var outpostStockLevelCurrentTreeModel = new OutpostStockLevelCurrentTreeModel() { Name = "root" };

            LoadUserAndClient();

            if (input.OutpostId.Equals(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST))
            {
                var outpostsThatHaveDistrictId = QueryOutpost.Query().Where(it => it.District.Id == input.DistrictId).ToList();

                foreach (var outpost in outpostsThatHaveDistrictId)
                {
                    var outpostStockLevelCurrentForOutpostId = QueryOutpostStockLevel.Query().Where(it => it.OutpostId == outpost.Id && it.Client.Id == _client.Id).ToList();
                    var treeModel = new OutpostStockLevelCurrentTreeModel();
                    treeModel.Name = outpost.Name;                                      

                        var outpostStockLevelCurrentGroupedByProductGroup = outpostStockLevelCurrentForOutpostId.OrderBy(it=>it.Updated).GroupBy(it => it.ProdGroupId).ToList();

                        foreach (var productGroup in outpostStockLevelCurrentGroupedByProductGroup)
                        {
                            var productGroupTreeModel = new OutpostStockLevelCurrentTreeModel();
                            productGroupTreeModel.Name = QueryProductGroup.Load(productGroup.Key).Name;

                            foreach (var product in productGroup)
                            {
                                var productTreeModel = new OutpostStockLevelCurrentTreeModel();
                                var productEntity = QueryProduct.Load(product.ProductId);
                                productTreeModel.Name = product.ProductName;
                                productTreeModel.Id = product.Id;
                                productTreeModel.LastUpdate = product.Updated.Value.ToShortDateString();
                                productTreeModel.leaf = true;
                                productTreeModel.PreviousLevel = product.PrevStockLevel;
                                productTreeModel.ProductLevel = product.StockLevel;
                                productTreeModel.SMSCode = product.ProdSmsRef;
                                productTreeModel.UpdateMethod = product.UpdateMethod;
                                productTreeModel.Description = productEntity.Description;
                                productTreeModel.ProductGroupName = productGroupTreeModel.Name;
                                productTreeModel.OutpostName = treeModel.Name;
                              
                                productGroupTreeModel.children.Add(productTreeModel);
                            }

                            if ((input.ProductGroupExpandedOnEdit != null) && (input.ProductGroupExpandedOnEdit.Equals(productGroupTreeModel.Name)))
                            {
                                productGroupTreeModel.expanded = true;
                                treeModel.expanded = true;
                            }
                            treeModel.children.Add(productGroupTreeModel);

                        }
                        outpostStockLevelCurrentTreeModel.children.Add(treeModel);                   
                }               

            }
            else
            {
                var outpostStockLevelCurrent = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id && it.OutpostId == input.OutpostId).ToList().OrderBy(it=>it.Updated).GroupBy(pg => pg.ProdGroupId);

                var treeModel = new OutpostStockLevelCurrentTreeModel();
                treeModel.Name = QueryOutpost.Load(input.OutpostId).Name;

                foreach (var productGroup in outpostStockLevelCurrent)
                {
                    var productGroupTreeModel = new OutpostStockLevelCurrentTreeModel();
                    productGroupTreeModel.Name = QueryProductGroup.Load(productGroup.Key).Name;

                    foreach (var product in productGroup)
                    {
                        var productEntity = QueryProduct.Load(product.ProductId);
                        var productTreeModel = new OutpostStockLevelCurrentTreeModel();
                        productTreeModel.Name = product.ProductName;
                        productTreeModel.Id = product.Id;
                        productTreeModel.LastUpdate = product.Updated.Value.ToShortDateString();
                        productTreeModel.leaf = true;
                        productTreeModel.PreviousLevel = product.PrevStockLevel;
                        productTreeModel.ProductLevel = product.StockLevel;
                        productTreeModel.SMSCode = product.ProdSmsRef;
                        productTreeModel.UpdateMethod = product.UpdateMethod;
                        productTreeModel.Description = productEntity.Description;
                        productTreeModel.ProductGroupName = productGroupTreeModel.Name;
                        productTreeModel.OutpostName = treeModel.Name;
                        productGroupTreeModel.children.Add(productTreeModel);
                    }

                    if ((input.ProductGroupExpandedOnEdit != null) && (input.ProductGroupExpandedOnEdit.Equals(productGroupTreeModel.Name)))
                    {
                        productGroupTreeModel.expanded = true;
                        treeModel.expanded = true;
                    }
                    treeModel.children.Add(productGroupTreeModel);
                }
                outpostStockLevelCurrentTreeModel.children.Add(treeModel);                
            }

            return Json(outpostStockLevelCurrentTreeModel, JsonRequestBehavior.AllowGet);
 
        }

        [HttpPost]
        public JsonResult Edit(OutpostStockLevelInputModel outpostStockLevelInput)
        {
            var outpostStockLevel = QueryOutpostStockLevel.Load(outpostStockLevelInput.Id.Value);

            var outpostHistoricalStockLevel = SetOutpostHistoricalStockLevelFromPreviousOutpostStockLevelCurrentData(outpostStockLevel);

            outpostStockLevel.PrevStockLevel = outpostStockLevel.StockLevel;
            outpostStockLevel.StockLevel = outpostStockLevelInput.StockLevel;
            outpostStockLevel.UpdateMethod = UPDATE_METHOD_SYSTEM;

            SaveOrUpdateOutpostStockLevelHistorical.Execute(outpostHistoricalStockLevel);
            SaveOrUpdateOutpostStockLevel.Execute(outpostStockLevel);

            return Json(new JsonActionResponse { Status = "Success", Message = "Outpost Stock Level successfully updated !" }, JsonRequestBehavior.AllowGet);
 
        }
        public OutpostHistoricalStockLevel SetOutpostHistoricalStockLevelFromPreviousOutpostStockLevelCurrentData(OutpostStockLevel outpostStockLevel)
        {
            var outpostHistoricalStockLevel = new OutpostHistoricalStockLevel();
            outpostHistoricalStockLevel.OutpostId = outpostStockLevel.OutpostId;
            outpostHistoricalStockLevel.PrevStockLevel = outpostStockLevel.PrevStockLevel;
            outpostHistoricalStockLevel.ProdGroupId = outpostStockLevel.ProdGroupId;
            outpostHistoricalStockLevel.ProdSmsRef = outpostStockLevel.ProdSmsRef;
            outpostHistoricalStockLevel.ProductId = outpostStockLevel.ProductId;
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

            if(countryId !=null)
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
                regions = regionList
            ,
                TotalItems = regionList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistricts(Guid? regionId)
        {
            LoadUserAndClient();

            var districts = new List<District>();
            var districtList = new List<EntityModel>();

            if (regionId != null)
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
                districts = districtList
            ,
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

            if (districtId != null)
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
                outposts = outpostList
            ,
                TotalItems = outpostList.Count
            }, JsonRequestBehavior.AllowGet);

        }
       
    }
}
