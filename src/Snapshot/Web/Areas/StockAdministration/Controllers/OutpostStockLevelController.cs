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


        public ActionResult Overview()
        {
            var outpostStockLevelOverview = new OutpostStockLevelOverviewModel(QueryCountry);

            return View(outpostStockLevelOverview);
        }

        public PartialViewResult OverviewItemsStockLevel(Guid? OutpostId)
        {
            var outpostList = new OutpostList();
            var outpostWithProductGroups = new OutpostWithProductGroups();

            if (OutpostId != null)
            {
                var outpost = QueryOutpost.Load(OutpostId.Value);
                List<Product> productsForOutpost = new List<Product>();
                if (outpost != null)
                {
                    outpostWithProductGroups.Id = OutpostId.Value;
                    outpostWithProductGroups.Name = outpost.Name;

                    if (outpost.Products.Count > 0)
                    {
                        foreach (Product item in outpost.Products)
                        {
                            productsForOutpost.Add(item);
                        }
                    }

                }

                var productGroups = QueryProductGroup.Query();
                var products = QueryProduct.Query();

                if (productGroups.ToList().Count > 0)
                {
                    foreach (ProductGroup stockGroup in productGroups)
                    {
                        var productsForGroup = productsForOutpost.Where(it => it.ProductGroup.Id == stockGroup.Id).ToList();
                        if (productsForGroup.Count > 0)
                        {
                            var productGroupWithItems = new ProductGroupWithProducts();
                            productGroupWithItems.Id = stockGroup.Id;
                            productGroupWithItems.Name = stockGroup.Name;

                            foreach (Product item in productsForGroup)
                            {
                                var productModel = new ProductModel();
                                CreateMappings();
                                Mapper.Map(item, productModel);
                                productModel.LastUpdateAt = item.Updated.Value.ToShortDateString();
                                productGroupWithItems.StockItems.Add(productModel);
                                
                            }
                            outpostWithProductGroups.StockGroups.Add(productGroupWithItems);
                        }
                    }
                }
            }

            return PartialView(outpostWithProductGroups);

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
        }
    }
}
