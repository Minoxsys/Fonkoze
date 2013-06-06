using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.ReportRegionLevel;
using Web.Security;
using Web.Models.UserManager;
using Web.Models.Shared;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class ReportRegionLevelController : Controller
    {
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        private Client _client;
        private User _user;

        private const string NAME_ALL_OPTION = "All";
        private Guid ID_ALL_OPTION = Guid.Empty;

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
            var countryList = new List<LocationEntityModel>();

            countryList.Add(new LocationEntityModel { Name = NAME_ALL_OPTION, Id = ID_ALL_OPTION });

            var countries = QueryCountry.Query().Where(it => it.Client.Id == _client.Id);

            foreach (var country in countries)
            {
                countryList.Add(new LocationEntityModel { Id = country.Id, Name = country.Name });
            }

            return Json(new
            {
                countries = countryList,
                TotalItems = countryList.Count
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetRegions(Guid CountryId)
        {
            LoadUserAndClient();
            var regionList = new List<LocationEntityModel>();

            regionList.Add(new LocationEntityModel { Name = NAME_ALL_OPTION, Id = ID_ALL_OPTION });

            var regions = QueryRegion.Query().Where(it => it.Client.Id == _client.Id);

            if (CountryId != ID_ALL_OPTION)
                regions = regions.Where(it => it.Country.Id == CountryId);

            foreach (var region in regions)
            {
                regionList.Add(new LocationEntityModel { Id = region.Id, Name = region.Name });
            }

            return Json(new
            {
                regions = regionList,
                TotalItems = regionList.Count
            }, JsonRequestBehavior.AllowGet);

        }    
    
        public JsonResult GetReports(FilterInputModel inputModel)
        {
            
            LoadUserAndClient();

            var reportRegionLevel = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();

            if ((inputModel.RegionId != ID_ALL_OPTION) && (inputModel.RegionId != Guid.Empty))
            {
                reportRegionLevel = reportRegionLevel.Where(it => it.Outpost.Region.Id == inputModel.RegionId).ToList();
            }
            else
            {
                if (inputModel.CountryId != ID_ALL_OPTION)
                {
                    reportRegionLevel = reportRegionLevel.Where(it => it.Outpost.Country.Id == inputModel.CountryId).ToList();
                }
            }

            var reportRegionLevelTreeModel = GetReportRegionTreeModel(reportRegionLevel);
            return Json(reportRegionLevelTreeModel, JsonRequestBehavior.AllowGet);
        }

        private ReportRegionLevelTreeModel GetReportRegionTreeModel(List<OutpostStockLevel> reportRegionLevel)
        {
            var reportRegionLevelTreeModel = new ReportRegionLevelTreeModel { Name = "root" };
            var reportRegionLevelGroupByRegion = reportRegionLevel.GroupBy(it => it.Outpost.Region);

            foreach (var regionGroup in reportRegionLevelGroupByRegion)
            {
                var regionNode = ToRegionNode(regionGroup);
                reportRegionLevelTreeModel.children.Add(regionNode);
            }
            return reportRegionLevelTreeModel;
        }

        private ReportRegionLevelTreeModel ToRegionNode(IGrouping<Region, OutpostStockLevel> regionGroup)
        {
            var regionNode = new ReportRegionLevelTreeModel { Name = regionGroup.Key.Name, ProductLevelSum = "" };
            regionNode.Name += " ( Sellers:" + QueryOutpost.Query().Count(it => it.Region.Id == regionGroup.Key.Id) + " ) ";
            regionNode.Id = regionGroup.Key.Id;

            var groupByProductGroup = regionGroup.GroupBy(it => it.ProductGroup);

            foreach (var productGroup in groupByProductGroup)
            {
                var productGroupNode = ToProductGroupNode(productGroup);
                regionNode.children.Add(productGroupNode);
            }
            return regionNode;
        }

        private static ReportRegionLevelTreeModel ToProductGroupNode(IGrouping<ProductGroup, OutpostStockLevel> productGroup)
        {
            var productGroupNode = new ReportRegionLevelTreeModel { Name = productGroup.Key.Name, ProductLevelSum = "", Id = productGroup.Key.Id };

            var groupByProductOnProductGroup = productGroup.GroupBy(it => it.Product);

            foreach (var item in groupByProductOnProductGroup)
            {
                var leafNode = ToProductNode(item);
                productGroupNode.children.Add(leafNode);
                
            }
            return productGroupNode;
        }

        private static ReportRegionLevelTreeModel ToProductNode(IGrouping<Product, OutpostStockLevel> item)
        {
            var leafNode = new ReportRegionLevelTreeModel { Name = item.Key.Name, Id = item.Key.Id };
            int productLevelSum = 0;
            foreach (var product in item)
            {
                productLevelSum += product.StockLevel;
            }
            leafNode.leaf = true;
            leafNode.ProductLevelSum = productLevelSum.ToString();
            return leafNode;
        }

        [Requires(Permissions = "Report.View")]
        public ActionResult Overview(NotNullableFilterModel filter)
        {
            return View(filter);
        }

        [HttpGet]
        public string GetProductFields(Guid? countryId, Guid? regionId)
        {
            string productFields = "";
            LoadUserAndClient();
            List<OutpostStockLevel> oslList = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            if (regionId != null && regionId.Value != ID_ALL_OPTION)
            {
                oslList = oslList.Where(it => it.Outpost.Region.Id == regionId).ToList();
            }
            else
            {
                if (countryId != null && countryId != ID_ALL_OPTION)
                {
                    oslList = oslList.Where(it => it.Outpost.Country.Id == countryId).ToList();
                }
            }

            var groupedByProduct = oslList.GroupBy(p => p.Product);
            foreach (var pGroup in groupedByProduct)
            {
                if (!productFields.Contains(pGroup.Key.Name + ","))
                    productFields += pGroup.Key.Name + ",";
            }

            return productFields.Trim(',');


        }

        [HttpGet]
        public JsonResult GetProducts(Guid? productGroupId)
        {
            List<ReferenceModel> listOfProducts = new List<ReferenceModel>();

            if (!productGroupId.HasValue)
                return Json(new StoreOutputModel<ReferenceModel>
                {
                    Items = listOfProducts.ToArray(),
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);

            listOfProducts = GetAllProductsFor(productGroupId.Value);

            return Json(new StoreOutputModel<ReferenceModel>
            {
                Items = listOfProducts.ToArray(),
                TotalItems = listOfProducts.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public List<ReferenceModel> GetAllProductsFor(Guid productGroupId)
        {
            LoadUserAndClient();

            List<ReferenceModel> allProducts = new List<ReferenceModel>();

            var productsStockLevel = QueryOutpostStockLevel.Query()
                    .Where(it => it.Client.Id == _client.Id)
                    .Where(it => it.ProductGroup.Id == productGroupId);

            foreach (var product in productsStockLevel)
            {
                ReferenceModel model = new ReferenceModel() { Name = product.Product.Name, Id = product.Product.Id };
                if (!allProducts.Exists(it => it.Name == model.Name && it.Id == model.Id))
                    allProducts.Add(model);
            }

            return allProducts;
        }

        [HttpGet]
        public JsonResult GetDataForStackedBarChart(Guid? countryId, Guid? regionId)
        {
            List<RegionStackedBarChartModel> chartData = new List<RegionStackedBarChartModel>();
           
            if (!countryId.HasValue && !regionId.HasValue)
            {
                return Json(new StoreOutputModel<RegionStackedBarChartModel>
                {
                    Items = chartData.ToArray(),
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);
            }
            LoadUserAndClient();

            List<OutpostStockLevel> oslList = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            if (regionId != null && regionId.Value != ID_ALL_OPTION)
            {
                oslList = oslList.Where(it => it.Outpost.Region.Id == regionId).ToList();
            }
            else
            {
                if (countryId != null && countryId != ID_ALL_OPTION)
                {
                    oslList = oslList.Where(it => it.Outpost.Country.Id == countryId).ToList();
                }
            }

            var groupedByRegion = oslList.GroupBy(it => it.Outpost.Region);
            foreach (var reg in groupedByRegion)
            {
                var item = new RegionStackedBarChartModel() { RegionName = reg.Key.Name };
                var groupedByProduct = reg.GroupBy(it => it.Product);
                foreach (var prod in groupedByProduct)
                {
                    var p = new ProductStackedBarChartModel() { ProductName = prod.Key.Name};
                    p.StockLevel = GetTotalProductStock(prod);
                    item.Products.Add(p);
                }
               
                chartData.Add(item);
            }
            return Json(new StoreOutputModel<RegionStackedBarChartModel>
            {
                Items = chartData.ToArray(),
                TotalItems = 0
            }, JsonRequestBehavior.AllowGet);
        }

       

        private int GetTotalProductStock(IGrouping<Product, OutpostStockLevel> prod)
        {
            int total = 0;
            foreach (var osl in prod)
            {
                total += osl.StockLevel;
            }
            return total;
        }

     
    }
}
