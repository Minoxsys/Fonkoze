using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public ISaveOrUpdateCommand<OutpostStockLevel> SaveOrUpdateOutpostStockLevel { get; set; }

        const string GUID_FOR_ALL_OPTION_ON_OUPOST_LIST = "00000000-0000-0000-0000-000000000001";

        public ActionResult Overview(Guid? countryId, Guid? regionId, Guid? districtId, Guid? outpostId)
        {
            var outpostStockLevelOverview = new OutpostStockLevelOverviewModel(QueryCountry);            

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

                GetAllDataForOutpostsAndFillOutpostList(outpostId, districtId, outpostStockLevelOverview.OutpostList);

                return View(outpostStockLevelOverview);               
 
            }
        }

        private List<SelectListItem> GetAllOutpostSpecificTo(Guid districtId,Guid outpostId)
        {
            var outpost = QueryOutpost.Query().Where(it => it.District.Id == districtId).ToList();
            var outpostsList = new List<SelectListItem>();

            if (outpost.Count > 0)
            {
                foreach (Outpost outpostItem in outpost)
                {
                    outpostsList.Add(new SelectListItem { Text = outpostItem.Name, Value = outpostItem.Id.ToString(), Selected = outpostItem.Id == outpostId });
                }
 
            }
            if (outpostId == Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUPOST_LIST))
            {
                outpostsList.Add(new SelectListItem { Text = "All", Value = GUID_FOR_ALL_OPTION_ON_OUPOST_LIST, Selected = true });
            }
            else
            {
                outpostsList.Add(new SelectListItem { Text = "All", Value = GUID_FOR_ALL_OPTION_ON_OUPOST_LIST});
 
            }
            return outpostsList;
        }

        private List<SelectListItem> GetAllDistrictsSpecificTo(Guid regionId,Guid districtId)
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

        private List<SelectListItem> GetAllRegionsSpecificTo(Guid countryId,Guid regionId)
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

        private void GetAllDataForOutpostsAndFillOutpostList(Guid? OutpostId, Guid? DistrictId, OutpostList outpostList)
        {
            if (OutpostId != null)
            {
                //take all outpost
                if (OutpostId == Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUPOST_LIST))
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
                        for (int i = 0; i < currentDataSpecificToDistrictId.Count; i++)
                        {
                            var outpostWithProductGroups = new OutpostWithProductGroups();

                            //if (i == currentDataSpecificToDistrictId.Count) i--;

                            outpostWithProductGroups.Id = currentDataSpecificToDistrictId[i].OutpostId;
                            outpostWithProductGroups.Name = QueryOutpost.Load(currentDataSpecificToDistrictId[i].OutpostId).Name;

                            var allDataWithSameOutpost = new List<OutpostStockLevel>();

                            allDataWithSameOutpost = currentData.FindAll(it => it.OutpostId == currentDataSpecificToDistrictId[i].OutpostId).ToList();

                            currentDataSpecificToDistrictId.RemoveAll(it => it.OutpostId == currentDataSpecificToDistrictId[i].OutpostId);

                            for (int j = 0; j <= allDataWithSameOutpost.Count; j++)
                            {
                                var productGroupWithProducts = new ProductGroupWithProducts();
                                var allDataWithSameProductGroup = new List<OutpostStockLevel>();
                                if (j == allDataWithSameOutpost.Count) j--;
                                allDataWithSameProductGroup = allDataWithSameOutpost.FindAll(it => it.ProdGroupId == allDataWithSameOutpost[j].ProdGroupId).ToList();
                                productGroupWithProducts.Id = allDataWithSameOutpost[j].ProdGroupId;
                                productGroupWithProducts.Name = QueryProductGroup.Load(allDataWithSameOutpost[j].ProdGroupId).Name;

                                allDataWithSameOutpost.RemoveAll(it => it.ProdGroupId == allDataWithSameOutpost[j].ProdGroupId);


                                for (int index = 0; index < allDataWithSameProductGroup.Count; index++)
                                {
                                    CreateMappings();
                                    var product = new Product();
                                    product = QueryProduct.Load(allDataWithSameProductGroup[index].ProductId);
                                    var productModel = new ProductModel();

                                    Mapper.Map(product, productModel);
                                    FillProductModelWithCurrentStockLevelInformation(allDataWithSameProductGroup, index, productModel);
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
                    var outpostsSpecificToOutpostId = new List<OutpostStockLevel>();
                    var outpostWithProductGroups = new OutpostWithProductGroups();


                    outpostsSpecificToOutpostId = QueryOutpostStockLevel.Query().Where(it => it.OutpostId == OutpostId.Value).ToList();
                    outpostWithProductGroups.Id = OutpostId.Value;
                    outpostWithProductGroups.Name = QueryOutpost.Load(OutpostId.Value).Name;

                    for (int j = 0; j <= outpostsSpecificToOutpostId.Count; j++)
                    {
                        var allDataWithSameProductGroup = new List<OutpostStockLevel>();
                        var productGroupWithProducts = new ProductGroupWithProducts();
                        if (j == outpostsSpecificToOutpostId.Count) j--;
                        allDataWithSameProductGroup = outpostsSpecificToOutpostId.FindAll(it => it.ProdGroupId == outpostsSpecificToOutpostId[j].ProdGroupId).ToList();
                        productGroupWithProducts.Id = outpostsSpecificToOutpostId[j].ProdGroupId;
                        productGroupWithProducts.Name = QueryProductGroup.Load(outpostsSpecificToOutpostId[j].ProdGroupId).Name;

                        outpostsSpecificToOutpostId.RemoveAll(it => it.ProdGroupId == outpostsSpecificToOutpostId[j].ProdGroupId);


                        for (int index = 0; index < allDataWithSameProductGroup.Count; index++)
                        {
                            CreateMappings();
                            var product = new Product();
                            product = QueryProduct.Load(allDataWithSameProductGroup[index].ProductId);
                            var productModel = new ProductModel();

                            Mapper.Map(product, productModel);
                            FillProductModelWithCurrentStockLevelInformation(allDataWithSameProductGroup, index, productModel);
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
            outpostStockLevelModel.ProductName = product.Name;
            outpostStockLevelModel.EditAreCommingFromFilterByAllOutposts = EditAreCommingFromFilterByAll;

            var outpost = QueryOutpost.Load(outpostStockLevel.OutpostId);
            outpostStockLevelModel.CountryId = outpost.Country.Id;
            outpostStockLevelModel.RegionId = outpost.Region.Id;
            outpostStockLevelModel.DistrictId = outpost.District.Id;

            return View(outpostStockLevelModel);

        }

        [HttpPost]
        public ActionResult EditCurrentProductLevel(OutpostStockLevelInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var outpostStockLevelOutputModel = new OutpostStockLevelOutputModel();
                return View("EditCurrentProductLevel", outpostStockLevelOutputModel);
            }

            CreateMappings();
            var outpostStockLevel = new OutpostStockLevel();

            Mapper.Map(inputModel, outpostStockLevel);

            SaveOrUpdateOutpostStockLevel.Execute(outpostStockLevel);

            var outpost = QueryOutpost.Load(inputModel.OutpostId);

            Guid CountryId = outpost.Country.Id;
            Guid RegionId = outpost.Region.Id;
            Guid DistrictId = outpost.District.Id;
            Guid OutpostId = outpost.Id;

            if (inputModel.EditAreCommingFromFilterByAllOutposts)
            {
                return RedirectToAction("Overview", new { countryId = CountryId.ToString(), regionId = RegionId.ToString(), districtId = DistrictId.ToString(), outpostId = GUID_FOR_ALL_OPTION_ON_OUPOST_LIST });
            }
            else
            {
                return RedirectToAction("Overview", new { countryId = CountryId.ToString(), regionId = RegionId.ToString(), districtId = DistrictId.ToString(), outpostId = OutpostId.ToString() });
            }

        }
        private static void FillProductModelWithCurrentStockLevelInformation(List<OutpostStockLevel> allDataWithSameProductGroup, int index, ProductModel productModel)
        {
            productModel.PreviousStockLevel = allDataWithSameProductGroup[index].PrevStockLevel;
            productModel.StockLevel = allDataWithSameProductGroup[index].StockLevel;
<<<<<<< HEAD
            //productModel.UpdateMethod = allDataWithSameProductGroup[index].UpdateMethod;
=======
            productModel.UpdateMethod = allDataWithSameProductGroup[index].UpdateMethod;
>>>>>>> outposts
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
        }
    }
}
