using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using Domain;
using Core.Persistence;
using Web.Areas.StockAdministration.Models.Product;
using AutoMapper;

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

        const string GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = "00000000-0000-0000-0000-000000000001";

        public ActionResult Overview(Guid? countryId, Guid? regionId, Guid? districtId, Guid? outpostId, bool? CommingFromCurrentData)
        {
            var outpostStockLevelOverview = new OutpostStockLevelOverviewModel(QueryCountry);
            if (CommingFromCurrentData != null)
                outpostStockLevelOverview.CommingFromCurrentData = CommingFromCurrentData.Value;

            if ((countryId == null) && (regionId == null) && (districtId == null) && (outpostId == null))
            {
                return View(outpostStockLevelOverview);
            }
            else
            {
                
                outpostStockLevelOverview.Regions = GetAllRegionsSpecificTo(countryId.Value, regionId.Value);
                outpostStockLevelOverview.Districts = GetAllDistrictsSpecificTo(regionId.Value, districtId.Value);
                outpostStockLevelOverview.Outposts = GetAllOutpostSpecificTo(districtId.Value, outpostId.Value);
                outpostStockLevelOverview.Countries.Where(it => it.Value == countryId.ToString()).ToList()[0].Selected = true;
                if (CommingFromCurrentData.Value)
                {
                    GetAllDataForOutpostsAndFillOutpostList(outpostId, districtId, outpostStockLevelOverview.OutpostList);
                }
                else
                {
                    GetAllDataForOutpostsAndFillOutpostListWithHistoricalData(outpostId, districtId, outpostStockLevelOverview.OutpostList);
                }

                return View(outpostStockLevelOverview);

            }
        }

        private List<SelectListItem> GetAllOutpostSpecificTo(Guid districtId, Guid outpostId)
        {
            var outposts = QueryOutpost.Query().Where(it => it.District.Id == districtId).ToList();
            var outpostsList = new List<SelectListItem>();

            if (outposts.Count > 0)
            {
                foreach (Outpost outpostItem in outposts)
                {
                    outpostsList.Add(new SelectListItem { Text = outpostItem.Name, Value = outpostItem.Id.ToString(), Selected = outpostItem.Id == outpostId });
                }

            }
            if (outpostId == Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST))
            {
                outpostsList.Add(new SelectListItem { Text = "All", Value = GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST, Selected = true });
            }
            else
            {
                outpostsList.Add(new SelectListItem { Text = "All", Value = GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST });

            }
            return outpostsList;
        }

        private List<SelectListItem> GetAllDistrictsSpecificTo(Guid regionId, Guid districtId)
        {
            var districts = QueryDistrict.Query().Where(it => it.Region.Id == regionId).ToList();
            var districtList = new List<SelectListItem>();

            if (districts.Count > 0)
            {
                foreach (District districtItem in districts)
                {
                    districtList.Add(new SelectListItem { Text = districtItem.Name, Value = districtItem.Id.ToString(), Selected = districtItem.Id == districtId });
                }
            }
            return districtList;
        }

        private List<SelectListItem> GetAllRegionsSpecificTo(Guid countryId, Guid regionId)
        {
            var regions = QueryRegion.Query().Where(it => it.Country.Id == countryId).ToList();
            var regionsList = new List<SelectListItem>();

            if (regions.Count > 0)
            {
                foreach (Region regionItem in regions)
                {
                    regionsList.Add(new SelectListItem { Text = regionItem.Name, Value = regionItem.Id.ToString(), Selected = regionItem.Id == regionId });
                }
            }
            return regionsList;
        }

        public PartialViewResult OverviewItemsStockLevel(Guid? OutpostId, Guid? DistrictId)
        {
            var outpostList = new OutpostList();

            GetAllDataForOutpostsAndFillOutpostList(OutpostId, DistrictId, outpostList);

            return PartialView(outpostList);
        }

        public PartialViewResult OverviewItemsStockLevelHistorical(Guid? OutpostId, Guid? DistrictId)
        {
            var outpostList = new OutpostList();

            GetAllDataForOutpostsAndFillOutpostListWithHistoricalData(OutpostId, DistrictId, outpostList);

            return PartialView(outpostList);
        }
        private void GetAllDataForOutpostsAndFillOutpostListWithHistoricalData(Guid? OutpostId, Guid? DistrictId, OutpostList outpostList)
        {
            if (OutpostId != null)
            {
                //all
                if (OutpostId == Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST))
                {
                    var historicalData = new List<OutpostHistoricalStockLevel>();
                    var historicalDataSpecificToDistrict = new List<OutpostHistoricalStockLevel>();
                    var outpostsForDistrictId = new List<Outpost>();

                    outpostsForDistrictId = QueryOutpost.Query().Where(it => it.District.Id == DistrictId.Value).ToList();
                    historicalData = QueryOutpostStockLevelHistorical.Query().ToList();

                    if (historicalData.Count > 0)
                    {
                        for (int index = 0; index < historicalData.Count; index++)
                        {
                            if (outpostsForDistrictId.Exists(it => it.Id == historicalData[index].OutpostId))
                            {
                                historicalDataSpecificToDistrict.Add(historicalData[index]);
                            }

                        }

                        var historicalDataGroupedByOutpostName = historicalDataSpecificToDistrict.OrderByDescending(it => it.UpdateDate).GroupBy(it => it.OutpostId).ToList();

                        foreach (var outpostGroup in historicalDataGroupedByOutpostName)
                        {
                            var outpost = QueryOutpost.Load(outpostGroup.Key);
                            var outpostWithProductGroups = new OutpostWithProductGroups();
                            outpostWithProductGroups.Id = outpost.Id;
                            outpostWithProductGroups.Name = outpost.Name;

                            foreach (var group in outpostGroup)
                            {
                                var productGroupWithProducts = new ProductGroupWithProducts();
                                var productGroup = QueryProductGroup.Load(group.ProdGroupId);
                                productGroupWithProducts.Id = productGroup.Id;
                                productGroupWithProducts.Name = productGroup.Name;
                                productGroupWithProducts.NoProducts = QueryProduct.Query().Count(it => it.ProductGroup.Id == group.ProdGroupId);
                                productGroupWithProducts.UpdateDate = group.UpdateDate.Value.ToShortDateString();

                                if (group.Updated != null)
                                    productGroupWithProducts.LastUpdateAt = group.Updated.Value.ToShortDateString();
                                productGroupWithProducts.UpdateMethod = group.UpdateMethod;
                                productGroupWithProducts.OutpostStockLevelHistoricalId = group.Id;
                                outpostWithProductGroups.StockGroups.Add(productGroupWithProducts);
                            }
                            outpostList.Outposts.Add(outpostWithProductGroups);
                        }
                    }
                }
                else
                {
                    //just one outpost

                    var historicalData = QueryOutpostStockLevelHistorical.Query().Where(it => it.OutpostId == OutpostId).OrderByDescending(it => it.UpdateDate).ToList();

                    if (historicalData.Count > 0)
                    {
                        var outpostWithProductGroups = new OutpostWithProductGroups();
                        var outpost = QueryOutpost.Load(OutpostId.Value);

                        outpostWithProductGroups.Id = outpost.Id;
                        outpostWithProductGroups.Name = outpost.Name;
                        foreach (var group in historicalData)
                        {
                            var productGroupWithProducts = new ProductGroupWithProducts();
                            var productGroupEntity = QueryProductGroup.Load(group.ProdGroupId);
                            productGroupWithProducts.Id = productGroupEntity.Id;
                            productGroupWithProducts.Name = productGroupEntity.Name;
                            productGroupWithProducts.NoProducts = QueryProduct.Query().Count(it => it.ProductGroup.Id == group.ProdGroupId);
                            productGroupWithProducts.UpdateDate = group.UpdateDate.Value.ToShortDateString();
                            productGroupWithProducts.UpdateMethod = group.UpdateMethod;
                                if(group.Updated !=null)
                            productGroupWithProducts.LastUpdateAt = group.Updated.Value.ToShortDateString();
                            productGroupWithProducts.OutpostStockLevelHistoricalId = group.Id;
                            outpostWithProductGroups.StockGroups.Add(productGroupWithProducts);

                        }
                        outpostList.Outposts.Add(outpostWithProductGroups);
                    }
                }
            }

        }
        private void GetAllDataForOutpostsAndFillOutpostList(Guid? OutpostId, Guid? DistrictId, OutpostList outpostList)
        {
            if (OutpostId != null)
            {
                //take all outpost
                if (OutpostId == Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST))
                {
                    var currentData = new List<OutpostStockLevel>();
                    var currentDataSpecificToDistrictId = new List<OutpostStockLevel>();
                    var outpostsForDistrictId = new List<Outpost>();

                    outpostsForDistrictId = QueryOutpost.Query().Where(it => it.District.Id == DistrictId.Value).ToList();
                    currentData = QueryOutpostStockLevel.Query().ToList();

                    if (currentData.Count > 0)
                    {

                        for (int index = 0; index < currentData.Count; index++)
                        {
                            if (outpostsForDistrictId.Exists(it => it.Id == currentData[index].OutpostId))
                            {
                                currentDataSpecificToDistrictId.Add(currentData[index]);
                            }

                        }

                        var currentDataGroupedByOutpostID = currentDataSpecificToDistrictId.GroupBy(it => it.OutpostId).ToList();

                        foreach (var group in currentDataGroupedByOutpostID)
                        {
                            var outpostWithProductGroups = new OutpostWithProductGroups();
                            var outpost = QueryOutpost.Load(group.Key);
                            outpostWithProductGroups.Id = outpost.Id;
                            outpostWithProductGroups.Name = outpost.Name;

                            var currentDataGroupedByProductGroupIdOnSameOutpost = currentDataSpecificToDistrictId.Where(it => it.OutpostId == group.Key).GroupBy(it => it.ProdGroupId).ToList();

                            foreach (var productGroup in currentDataGroupedByProductGroupIdOnSameOutpost)
                            {
                                var productGroupWithProducts = new ProductGroupWithProducts();
                                var productGroupEntity = QueryProductGroup.Load(productGroup.Key);
                                productGroupWithProducts.Id = productGroupEntity.Id;
                                productGroupWithProducts.Name = productGroupEntity.Name;

                                foreach (var product in productGroup)
                                {
                                    CreateMappings();
                                                                       
                                    var productModel = new ProductModel();
                                    var productEntity = QueryProduct.Load(product.ProductId);
                                    Mapper.Map(productEntity, productModel);

                                    if (product.Updated != null)
                                        productModel.LastUpdateAt = product.Updated.Value.ToShortDateString();
                                    productModel.PreviousStockLevel = product.PrevStockLevel;
                                    productModel.StockLevel = product.StockLevel;
                                    productModel.UpdateMethod = product.UpdatedMethod;
                                    productModel.OutpostStockLevelId = product.Id;

                                    productGroupWithProducts.StockItems.Add(productModel);
                                }
                                outpostWithProductGroups.StockGroups.Add(productGroupWithProducts);

                            }
                            outpostList.Outposts.Add(outpostWithProductGroups);
                        }

                    }
                }
                else
                {
                    var outpostWithProductGroups = new OutpostWithProductGroups();

                    var outpostsSpecificToOutpostIdGroupedByProductGroups = QueryOutpostStockLevel.Query().Where(it => it.OutpostId == OutpostId.Value).ToList();//.GroupBy(it => it.ProdGroupId).ToList();
                    var outpostsGroupedByProductGroups = outpostsSpecificToOutpostIdGroupedByProductGroups.GroupBy(it => it.ProdGroupId).ToList();
                    outpostWithProductGroups.Id = OutpostId.Value;
                    outpostWithProductGroups.Name = QueryOutpost.Load(OutpostId.Value).Name;

                    foreach (var productGroup in outpostsGroupedByProductGroups)
                    {
                        var productGroupWithProducts = new ProductGroupWithProducts();

                        var productGroupEntity = QueryProductGroup.Load(productGroup.Key);
                        productGroupWithProducts.Id = productGroupEntity.Id;
                        productGroupWithProducts.Name = productGroupEntity.Name;

                        foreach (var productItem in productGroup)
                        {
                            CreateMappings();         
                            var productModel = new ProductModel();
                            var productEntity = QueryProduct.Load(productItem.ProductId);
                            Mapper.Map(productEntity, productModel);

                            if (productItem.Updated != null)
                                productModel.LastUpdateAt = productItem.Updated.Value.ToShortDateString();
                            productModel.PreviousStockLevel = productItem.PrevStockLevel;
                            productModel.StockLevel = productItem.StockLevel;
                            productModel.UpdateMethod = productItem.UpdatedMethod;
                            productModel.OutpostStockLevelId = productItem.Id;
                            productGroupWithProducts.StockItems.Add(productModel);

                        }
                        outpostWithProductGroups.StockGroups.Add(productGroupWithProducts);
                    }
                    outpostList.Outposts.Add(outpostWithProductGroups);
                }

            }
        }

        public ActionResult EditCurrentProductLevel(Guid OutpostStockLevelId, bool EditAreCommingFromFilterByAll)
        {
            var outpostStockLevel = new OutpostStockLevel();
            outpostStockLevel = QueryOutpostStockLevel.Load(OutpostStockLevelId);

            var outpostStockLevelModel = new OutpostStockLevelOutputModel();

            CreateMappings();
            Mapper.Map(outpostStockLevel, outpostStockLevelModel);

            outpostStockLevelModel.OutpostName = QueryOutpost.Load(outpostStockLevel.OutpostId).Name;
            outpostStockLevelModel.ProductGroupName = QueryProductGroup.Load(outpostStockLevel.ProdGroupId).Name;
            var product = QueryProduct.Load(outpostStockLevel.ProductId);
            outpostStockLevelModel.ProductDescription = product.Description;
            outpostStockLevelModel.ProductName = outpostStockLevel.ProductName;
            outpostStockLevelModel.EditAreCommingFromFilterByAllOutposts = EditAreCommingFromFilterByAll;

            var outpost = QueryOutpost.Load(outpostStockLevel.OutpostId);
            outpostStockLevelModel.CountryId = outpost.Country.Id;
            outpostStockLevelModel.RegionId = outpost.Region.Id;
            outpostStockLevelModel.DistrictId = outpost.District.Id;

            return View(outpostStockLevelModel);

        }

        public ActionResult ViewProductsForProductGroupFromHistoricalOverview(Guid OutpostStockLevelHistoricalId, bool? AreCommingFromFilterByAll)
        {
            var outpostStockLevelHistorical = QueryOutpostStockLevelHistorical.Load(OutpostStockLevelHistoricalId);
            var outpost = QueryOutpost.Load(outpostStockLevelHistorical.OutpostId);
            var productGroup = QueryProductGroup.Load(outpostStockLevelHistorical.ProdGroupId);

            var productGroupOverviewModel = new ProductGroupOverviewModel();

            productGroupOverviewModel.CountryId = outpost.Country.Id;
            productGroupOverviewModel.DistrictId = outpost.District.Id;
            productGroupOverviewModel.RegionId = outpost.Region.Id;
            productGroupOverviewModel.OutpostId = outpost.Id;
            productGroupOverviewModel.OutpostName = outpost.Name;
            productGroupOverviewModel.ProductGroupId = productGroup.Id;
            productGroupOverviewModel.ProductGroupName = productGroup.Name;
            if(AreCommingFromFilterByAll !=null)
            productGroupOverviewModel.AreCommingFromFilterByAll = AreCommingFromFilterByAll.Value;

            var productsOfProductGroupOnUpdateDate = QueryOutpostStockLevelHistorical.Query().Where(it => it.ProdGroupId == outpostStockLevelHistorical.ProdGroupId).ToList();

            if (productsOfProductGroupOnUpdateDate.Count > 0)
            {
                foreach (var item in productsOfProductGroupOnUpdateDate)
                {
                    if (item.Updated.Value.ToShortDateString().Equals(outpostStockLevelHistorical.Updated.Value.ToShortDateString()))
                    {
                        var productModel = new ProductModel();
                        var product = QueryProduct.Load(item.ProductId);

                        CreateMappings();
                        Mapper.Map(product, productModel);
                        productModel.StockLevel = item.StockLevel;
                        productModel.LastUpdateAt = outpostStockLevelHistorical.Updated.Value.ToShortDateString();
                        productModel.OutpostStockLevelId = item.Id;
                        productGroupOverviewModel.Products.Add(productModel);
                    }
                }

            }

            return View(productGroupOverviewModel);
        }

        public ActionResult EditHistoricalProductLevel(Guid OutpostStockLevelHistoricalId, bool AreCommingFromFilterByAll)
        {
            var outpostStockLevelHistorical = QueryOutpostStockLevelHistorical.Load(OutpostStockLevelHistoricalId);
            var outpost = QueryOutpost.Load(outpostStockLevelHistorical.OutpostId);
            var productGroup = QueryProductGroup.Load(outpostStockLevelHistorical.ProdGroupId);
            var product = QueryProduct.Load(outpostStockLevelHistorical.ProductId);

            var outpostStockLevelHistoricalOutputModel = new OutpostStockLevelHistoricalOutputModel();

            CreateMappings();
            Mapper.Map(outpostStockLevelHistorical, outpostStockLevelHistoricalOutputModel);

            outpostStockLevelHistoricalOutputModel.AreCommingFromFilterByAll = AreCommingFromFilterByAll;
            outpostStockLevelHistoricalOutputModel.CountryId = outpost.Country.Id;
            outpostStockLevelHistoricalOutputModel.DistrictId = outpost.District.Id;
            outpostStockLevelHistoricalOutputModel.RegionId = outpost.Region.Id;
            outpostStockLevelHistoricalOutputModel.OutpostId = outpost.Id;
            outpostStockLevelHistoricalOutputModel.OutpostName = outpost.Name;
            outpostStockLevelHistoricalOutputModel.ProductGroupName = productGroup.Name;
            outpostStockLevelHistoricalOutputModel.ProductDescription = product.Description;
            outpostStockLevelHistoricalOutputModel.ProductName = product.Name;
            if (outpostStockLevelHistorical.Updated != null)
                outpostStockLevelHistoricalOutputModel.LastUpdateAt = outpostStockLevelHistorical.Updated.Value.ToShortDateString();

            return View(outpostStockLevelHistoricalOutputModel);
        }

        [HttpPost]
        public ActionResult EditHistoricalProductLevel(OutpostStockLevelHistoricalInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var outputModel = FillOutputModelWithDataFromInputModel(inputModel);

                return View("EditHistoricalProductLevel", outputModel);
            }

            var outpostStockLevelHistorical = QueryOutpostStockLevelHistorical.Load(inputModel.Id);

            outpostStockLevelHistorical.PrevStockLevel = outpostStockLevelHistorical.StockLevel;
            outpostStockLevelHistorical.StockLevel = inputModel.StockLevel;

            SaveOrUpdateOutpostStockLevelHistorical.Execute(outpostStockLevelHistorical);


            return RedirectToAction("ViewProductsForProductGroupFromHistoricalOverview", new { OutpostStockLevelHistoricalId = inputModel.Id, AreCommingFromFilterByAll = inputModel.AreCommingFromFilterByAll });

        }

        private static OutpostStockLevelHistoricalOutputModel FillOutputModelWithDataFromInputModel(OutpostStockLevelHistoricalInputModel inputModel)
        {
            var outputModel = new OutpostStockLevelHistoricalOutputModel();
            outputModel.AreCommingFromFilterByAll = inputModel.AreCommingFromFilterByAll;
            outputModel.Id = inputModel.Id;
            outputModel.OutpostId = inputModel.OutpostId;
            outputModel.PrevStockLevel = inputModel.PrevStockLevel;
            outputModel.ProdGroupId = inputModel.ProdGroupId;
            outputModel.ProdSmsRef = inputModel.ProdSmsRef;
            outputModel.ProductId = inputModel.ProductId;
            outputModel.OutpostName = inputModel.OutpostName;
            outputModel.ProductDescription = inputModel.ProductDescription;
            outputModel.ProductGroupName = inputModel.ProductGroupName;
            outputModel.ProductName = inputModel.ProductName;
            outputModel.StockLevel = inputModel.StockLevel;
            outputModel.UpdateDate = inputModel.UpdateDate;
            outputModel.UpdateMethod = inputModel.UpdateMethod;
            return outputModel;
        }

        [HttpPost]
        public ActionResult EditCurrentProductLevel(OutpostStockLevelInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var outpostStockLevelOutputModel = BuildOutpostStockLevelOutputModelFromInputModel(inputModel);

                return View("EditCurrentProductLevel", outpostStockLevelOutputModel);
            }

            var currentOutpostStockLevel = QueryOutpostStockLevel.Load(inputModel.Id);
            var outpostStockLevelHistorical = SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(currentOutpostStockLevel);

            currentOutpostStockLevel.PrevStockLevel = currentOutpostStockLevel.StockLevel;
            currentOutpostStockLevel.StockLevel = inputModel.StockLevel;

            currentOutpostStockLevel.UpdatedMethod = "manually";


            SaveOrUpdateOutpostStockLevelHistorical.Execute(outpostStockLevelHistorical);
            SaveOrUpdateOutpostStockLevel.Execute(currentOutpostStockLevel);

            var outpost = QueryOutpost.Load(inputModel.OutpostId);

            Guid CountryId = outpost.Country.Id;
            Guid RegionId = outpost.Region.Id;
            Guid DistrictId = outpost.District.Id;
            Guid OutpostId = outpost.Id;

            if (inputModel.EditAreCommingFromFilterByAllOutposts)
            {
                return RedirectToAction("Overview", new { countryId = CountryId.ToString(), regionId = RegionId.ToString(), districtId = DistrictId.ToString(), outpostId = GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST, CommingFromCurrentData = true });
            }
            else
            {
                return RedirectToAction("Overview", new { countryId = CountryId.ToString(), regionId = RegionId.ToString(), districtId = DistrictId.ToString(), outpostId = OutpostId.ToString(), CommingFromCurrentData = true });
            }

        }


        private static OutpostHistoricalStockLevel SetHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(OutpostStockLevel previousOutpostStockLevel)
        {
            var outpostStockLevelHistorical = new OutpostHistoricalStockLevel();
            outpostStockLevelHistorical.OutpostId = previousOutpostStockLevel.OutpostId;
            outpostStockLevelHistorical.PrevStockLevel = previousOutpostStockLevel.PrevStockLevel;
            outpostStockLevelHistorical.ProdGroupId = previousOutpostStockLevel.ProdGroupId;
            outpostStockLevelHistorical.ProdSmsRef = previousOutpostStockLevel.ProdSmsRef;
            outpostStockLevelHistorical.ProductId = previousOutpostStockLevel.ProductId;
            outpostStockLevelHistorical.StockLevel = previousOutpostStockLevel.StockLevel;
            if (previousOutpostStockLevel.Updated != null)
            {
                outpostStockLevelHistorical.UpdateDate = previousOutpostStockLevel.Updated.Value;
            }
            else
            {
                outpostStockLevelHistorical.UpdateDate = DateTime.Now;
            }
            outpostStockLevelHistorical.UpdateMethod = previousOutpostStockLevel.UpdatedMethod;
            return outpostStockLevelHistorical;
        }

        private static OutpostStockLevelOutputModel BuildOutpostStockLevelOutputModelFromInputModel(OutpostStockLevelInputModel inputModel)
        {
            var outpostStockLevelOutputModel = new OutpostStockLevelOutputModel();
            outpostStockLevelOutputModel.EditAreCommingFromFilterByAllOutposts = inputModel.EditAreCommingFromFilterByAllOutposts;
            outpostStockLevelOutputModel.Id = inputModel.Id;
            outpostStockLevelOutputModel.OutpostId = inputModel.OutpostId;
            outpostStockLevelOutputModel.OutpostName = inputModel.OutpostName;
            outpostStockLevelOutputModel.PrevStockLevel = inputModel.PrevStockLevel;
            outpostStockLevelOutputModel.ProdGroupId = inputModel.ProdGroupId;
            outpostStockLevelOutputModel.ProdSmsRef = inputModel.ProdSmsRef;
            outpostStockLevelOutputModel.ProductDescription = inputModel.ProductDescription;
            outpostStockLevelOutputModel.ProductGroupName = inputModel.ProductGroupName;
            outpostStockLevelOutputModel.ProductId = inputModel.ProductId;
            outpostStockLevelOutputModel.ProductName = inputModel.ProductName;
            outpostStockLevelOutputModel.StockLevel = inputModel.StockLevel;
            outpostStockLevelOutputModel.UpdateMethod = inputModel.UpdateMethod;
            return outpostStockLevelOutputModel;
        }
        private static void FillProductModelWithCurrentStockLevelInformation(List<OutpostStockLevel> allDataWithSameProductGroup, int index, ProductModel productModel)
        {
            productModel.PreviousStockLevel = allDataWithSameProductGroup[index].PrevStockLevel;
            productModel.StockLevel = allDataWithSameProductGroup[index].StockLevel;
            productModel.UpdateMethod = allDataWithSameProductGroup[index].UpdatedMethod;
            productModel.OutpostStockLevelId = allDataWithSameProductGroup[index].Id;
            if (allDataWithSameProductGroup[index].Updated != null)
                productModel.LastUpdateAt = allDataWithSameProductGroup[index].Updated.Value.ToShortDateString();
        }
        [HttpGet]
        public JsonResult GetRegionsForCountry(Guid? countryId)
        {
            List<SelectListItem> Regions = new List<SelectListItem>();

            var regions = QueryRegion.Query().Where(it => it.Country.Id == countryId.Value);

            if (regions.ToList().Count > 0)
            {
                foreach (var item in regions)
                {
                    Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Regions;

            return Json(Regions, JsonRequestBehavior.AllowGet);

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

            var outposts = QueryOutpost.Query().Where(it => it.District.Id == districtId.Value);

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

        private void CreateMappings()
        {
            Mapper.CreateMap<ProductModel, Product>();
            Mapper.CreateMap<Product, ProductModel>();

            Mapper.CreateMap<ProductGroup, ProductGroupModel>();
            Mapper.CreateMap<ProductGroupModel, ProductGroup>();

            Mapper.CreateMap<OutpostStockLevelOutputModel, OutpostStockLevel>();
            Mapper.CreateMap<OutpostStockLevel, OutpostStockLevelOutputModel>();

            Mapper.CreateMap<OutpostStockLevelInputModel, OutpostStockLevel>();
            Mapper.CreateMap<OutpostStockLevel, OutpostStockLevelInputModel>();

            Mapper.CreateMap<OutpostStockLevelHistoricalOutputModel, OutpostHistoricalStockLevel>();
            Mapper.CreateMap<OutpostHistoricalStockLevel, OutpostStockLevelHistoricalOutputModel>();

            Mapper.CreateMap<OutpostStockLevelHistoricalInputModel, OutpostHistoricalStockLevel>();
            Mapper.CreateMap<OutpostHistoricalStockLevel, OutpostStockLevelHistoricalInputModel>();
        }
    }
}
