﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.ReportRegionLevel;
using Web.Areas.AnalysisManagement.Models.ReportDistrictLevel;
using Web.Security;
using Web.Models.Shared;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class ReportDistrictLevelController : Controller
    {
        public IQueryService<District> QueryDistrict { get; set; }
        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }
        public IQueryService<User> QueryUser { get; set; }
        public IQueryService<Client> QueryClient { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        private Client _client;
        private User _user;

        private const string NAME_ALL_OPTION = "All";
        private Guid ID_ALL_OPTION = Guid.Empty;
        
        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUser.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClient.Load(clientId);
        }

        public JsonResult GetDistricts(Guid? CountryId, Guid? RegionId)
        {
            LoadUserAndClient();

            var districtList = new List<LocationEntityModel>();
            districtList.Add(new LocationEntityModel { Name = NAME_ALL_OPTION, Id = ID_ALL_OPTION });

            var districts = QueryDistrict.Query().Where(it => it.Client.Id == _client.Id);

            if (RegionId.HasValue && (RegionId.Value != ID_ALL_OPTION))
            {
                districts = districts.Where(it => it.Region.Id == RegionId);
            }
            else
            {
                if (CountryId.HasValue && (CountryId.Value != ID_ALL_OPTION))
                    districts = districts.Where(it => it.Region.Country.Id == CountryId);
            }

            foreach (var district in districts)
            {
                districtList.Add(new LocationEntityModel { Name = district.Name, Id = district.Id });
            }

            return Json(new
            {
                districts = districtList,
                TotalItems = districtList.Count
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetReports(ReportDistrictLevelInputModel inputModel)
        {
            LoadUserAndClient();

            var reportDistrictLevel = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();

            if ((inputModel.DistrictId != ID_ALL_OPTION) && (inputModel.DistrictId != Guid.Empty))
            {
                reportDistrictLevel = reportDistrictLevel.Where(it => it.Outpost.District.Id == inputModel.DistrictId).ToList();
            }
            else
            {
                if ((inputModel.RegionId != ID_ALL_OPTION) && (inputModel.RegionId != Guid.Empty))
                {
                    reportDistrictLevel = reportDistrictLevel.Where(it => it.Outpost.Region.Id == inputModel.RegionId).ToList();
                }
                else
                {
                    if ((inputModel.CountryId != ID_ALL_OPTION) && (inputModel.CountryId != Guid.Empty))
                    {
                        reportDistrictLevel = reportDistrictLevel.Where(it => it.Outpost.Country.Id == inputModel.CountryId).ToList();
                    }
                }
            }

            var reportDistrictLevelTreeModel = GetReportRegionTreeModel(reportDistrictLevel); 
            return Json(reportDistrictLevelTreeModel, JsonRequestBehavior.AllowGet);
        }

        private ReportDistrictLevelTreeModel GetReportRegionTreeModel(List<OutpostStockLevel> reportDistrictLevel)
        {
            var reportDistrictLevelTreeModel = new ReportDistrictLevelTreeModel();
            var reportDistrictLevelGroupByDistrict = reportDistrictLevel.GroupBy(it => it.Outpost.District);

            foreach (var districtGroup in reportDistrictLevelGroupByDistrict)
            {
                var districtNode = ToDistrictNode(districtGroup);
                reportDistrictLevelTreeModel.children.Add(districtNode);
            }
            return reportDistrictLevelTreeModel;
        }

        private ReportDistrictLevelTreeModel ToDistrictNode(IGrouping<District, OutpostStockLevel> districtGroup)
        {
            var districtNode = new ReportDistrictLevelTreeModel { Name = districtGroup.Key.Name, ProductLevelSum = "", Id = districtGroup.Key.Id };
            districtNode.Name += " ( Sellers: " + QueryOutpost.Query().Count(it => it.District.Id == districtGroup.Key.Id) + " ) ";

            var groupByProductGroup = districtGroup.GroupBy(it => it.ProductGroup);

            foreach (var productGroupItem in groupByProductGroup)
            {
                var productGroupNode = ToProductGroupNode(productGroupItem, districtGroup.Key.Id);
                districtNode.children.Add(productGroupNode);

            }
            return districtNode;
        }

        private static ReportDistrictLevelTreeModel ToProductGroupNode(IGrouping<ProductGroup, OutpostStockLevel> productGroupItem, Guid parentId)
        {
            var productGroupNode = new ReportDistrictLevelTreeModel { Name = productGroupItem.Key.Name, ProductLevelSum = "", Id = productGroupItem.Key.Id, ParentId = parentId };

            var groupByProduct = productGroupItem.GroupBy(it => it.Product);

            foreach (var product in groupByProduct)
            {
                var productNode = ToProductNode(product, productGroupItem.Key.Id);

                productGroupNode.children.Add(productNode);
            }
            return productGroupNode;
        }

        private static ReportDistrictLevelTreeModel ToProductNode(IGrouping<Product, OutpostStockLevel> product, Guid parentId)
        {
            var productNode = new ReportDistrictLevelTreeModel { Name = product.Key.Name, ProductLevelSum = "", Id = product.Key.Id, ParentId = parentId };

            int productLevelSum = 0;

            foreach (var item in product)
            {
                productLevelSum += item.StockLevel;
            }
            productNode.ProductLevelSum = productLevelSum.ToString();
            productNode.leaf = true;
            return productNode;
        }

        [Requires(Permissions = "Report.View")]
        public ActionResult Overview(NotNullableFilterModel filter)
        {
            return View(filter);
        }

        public JsonResult GetProductsForChart(Guid? productGroupId, Guid? districtId) 
        {
            List<ProductsChartModel> listOfProducts = new List<ProductsChartModel>();

            if (!productGroupId.HasValue || !districtId.HasValue)
                return Json(new StoreOutputModel<ProductsChartModel>
                {
                    Items = listOfProducts.ToArray(),
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);

            listOfProducts = GetProductsAssociatedTo(productGroupId.Value, districtId.Value);
            
            return Json(new StoreOutputModel<ProductsChartModel>
            {
                Items = listOfProducts.ToArray(),
                TotalItems = listOfProducts.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<ProductsChartModel> GetProductsAssociatedTo(Guid productGroupId, Guid districtId)
        {
            LoadUserAndClient();
            List<ProductsChartModel> listOfProducts = new List<ProductsChartModel>();

            var reportDistrictLevel = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            var reportDistrictLevelTreeModel = GetReportRegionTreeModel(reportDistrictLevel);

            var district = reportDistrictLevelTreeModel.children.Find(it => it.Id == districtId);
            var pg = district.children.Find(it => it.Id == productGroupId);
            foreach (var product in pg.children)
            {
                ProductsChartModel model = new ProductsChartModel() { ProductName = product.Name, StockLevel = product.ProductLevelSum };
                listOfProducts.Add(model);
            }

            return listOfProducts;
        }

        [HttpGet]
        public string GetProductFields(Guid? countryId, Guid? regionId,Guid? districtId)
        {
            string productFields = "";
            LoadUserAndClient();
            List<OutpostStockLevel> oslList = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            if (districtId != null && districtId.Value != ID_ALL_OPTION)
            {
                oslList = oslList.Where(it => it.Outpost.District.Id == districtId).ToList();
            }
            else
            {
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
        public JsonResult GetDataForStackedBarChart(Guid? countryId, Guid? regionId, Guid? districtId)
        {
            List<DistrictStackedBarChartModel> chartData = new List<DistrictStackedBarChartModel>();

            if (!countryId.HasValue && !regionId.HasValue)
            {
                return Json(new StoreOutputModel<DistrictStackedBarChartModel>
                {
                    Items = chartData.ToArray(),
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);
            }
            LoadUserAndClient();

            List<OutpostStockLevel> oslList = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            if (districtId != null && districtId.Value != ID_ALL_OPTION)
            {
                oslList = oslList.Where(it => it.Outpost.District.Id == districtId).ToList();
            }
            else
            {
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
            }
            var groupedByDistrict = oslList.GroupBy(it => it.Outpost.District);
            foreach (var reg in groupedByDistrict)
            {
                var item = new DistrictStackedBarChartModel() { DistrictName = reg.Key.Name };
                var groupedByProduct = reg.GroupBy(it => it.Product);
                foreach (var prod in groupedByProduct)
                {
                    var p = new ProductStackedBarChartModel() { ProductName = prod.Key.Name };
                    p.StockLevel = GetTotalProductStock(prod);
                    item.Products.Add(p);
                }

                chartData.Add(item);
            }
            return Json(new StoreOutputModel<DistrictStackedBarChartModel>
            {
                Items = chartData.ToArray(),
                TotalItems = chartData.Count()
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
