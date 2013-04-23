using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Core.Persistence;
using Domain;
using Web.Areas.AnalysisManagement.Models.ReportDistrictLevel;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;


namespace Web.Areas.AnalysisManagement.Controllers
{
    public class ReportOutpostLevelController : Controller
    {
        public IQueryService<District> QueryDistrict { get; set; }
        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }
        public IQueryService<User> QueryUser { get; set; }
        public IQueryService<Client> QueryClient { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        private Client _client;
        private User _user;

        private const string NAME_ALL_OPTION = "All";
        private Guid ID_ALL_OPTION_FOR_COUNTRIES = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private Guid ID_ALL_OPTION_FOR_REGIONS = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private Guid ID_ALL_OPTION_FOR_DISTRICTS = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private Guid ID_ALL_OPTION_FOR_OUTPOSTS = Guid.Parse("00000000-0000-0000-0000-000000000004");

        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetReports(ReportOutpostLevelInputModel inputModel)
        {
            LoadUserAndClient();

            var reportOutpostLevel = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            if ((inputModel.OutpostId != ID_ALL_OPTION_FOR_OUTPOSTS) && (inputModel.OutpostId != Guid.Empty))
            {
                reportOutpostLevel = reportOutpostLevel.Where(it => it.Outpost.Id == inputModel.OutpostId).ToList();
            }
            else
            {
                if ((inputModel.DistrictId != ID_ALL_OPTION_FOR_DISTRICTS) && (inputModel.DistrictId != Guid.Empty))
                {
                    reportOutpostLevel = reportOutpostLevel.Where(it => it.Outpost.District.Id == inputModel.DistrictId).ToList();
                }
                else
                {
                    if ((inputModel.RegionId != ID_ALL_OPTION_FOR_REGIONS) && (inputModel.RegionId != Guid.Empty))
                    {
                        reportOutpostLevel = reportOutpostLevel.Where(it => it.Outpost.Region.Id == inputModel.RegionId).ToList();
                    }
                    else
                    {
                        if ((inputModel.CountryId != ID_ALL_OPTION_FOR_COUNTRIES) && (inputModel.CountryId != Guid.Empty))
                        {
                            reportOutpostLevel = reportOutpostLevel.Where(it => it.Outpost.Country.Id == inputModel.CountryId).ToList();
                        }
                    }
                }
            }

            var reportDistrictLevelTreeModel = GetReportOutpostTreeModel(reportOutpostLevel);
            return Json(reportDistrictLevelTreeModel, JsonRequestBehavior.AllowGet);
        }

        private ReportOutpostLevelTreeModel GetReportOutpostTreeModel(List<OutpostStockLevel> reportOutpostLevel)
        {
            var reportOutpostLevelTreeModel = new ReportOutpostLevelTreeModel();
            var reportOutpostLevelGroupByOutpost = reportOutpostLevel.GroupBy(it => it.Outpost);

            foreach (var outpostGroup in reportOutpostLevelGroupByOutpost)
            {
                var outpostNode = ToOutpostNode(outpostGroup);
                reportOutpostLevelTreeModel.children.Add(outpostNode);
            }
            return reportOutpostLevelTreeModel;
        }

        private ReportOutpostLevelTreeModel ToOutpostNode(IGrouping<Outpost, OutpostStockLevel> outpostGroup)
        {
            var outpostNode = new ReportOutpostLevelTreeModel { Name = outpostGroup.Key.Name, ProductLevelSum = "", Id = outpostGroup.Key.Id };
           // districtNode.Name += " ( Number of Sellers: " + QueryOutpost.Query().Count(it => it.District.Id == districtGroup.Key.Id) + " ) ";

            var groupByProductGroup = outpostGroup.GroupBy(it => it.ProductGroup);

            foreach (var productGroupItem in groupByProductGroup)
            {
                var productGroupNode = ToProductGroupNode(productGroupItem, outpostGroup.Key.Id);
                outpostNode.children.Add(productGroupNode);

            }
            return outpostNode;
        }

        private static ReportOutpostLevelTreeModel ToProductGroupNode(IGrouping<ProductGroup, OutpostStockLevel> productGroupItem, Guid parentId)
        {
            var productGroupNode = new ReportOutpostLevelTreeModel { Name = productGroupItem.Key.Name, ProductLevelSum = "", Id = productGroupItem.Key.Id, ParentId = parentId };

            var groupByProduct = productGroupItem.GroupBy(it => it.Product);

            foreach (var product in groupByProduct)
            {
                var productNode = ToProductNode(product, productGroupItem.Key.Id);

                productGroupNode.children.Add(productNode);
            }
            return productGroupNode;
        }

        private static ReportOutpostLevelTreeModel ToProductNode(IGrouping<Product, OutpostStockLevel> product, Guid parentId)
        {
            var productNode = new ReportOutpostLevelTreeModel { Name = product.Key.Name, ProductLevelSum = "", Id = product.Key.Id, ParentId = parentId };

            int productLevelSum = 0;

            foreach (var item in product)
            {
                productLevelSum += item.StockLevel;
            }
            productNode.ProductLevelSum = productLevelSum.ToString();
            productNode.leaf = true;
            return productNode;
        }

        public JsonResult GetProductsForChart(Guid? productGroupId, Guid? outpostId)
        {
            List<ProductsChartModel> listOfProducts = new List<ProductsChartModel>();

            if (!productGroupId.HasValue || !outpostId.HasValue)
                return Json(new ProductsForChartOutputModel
                {
                    Products = listOfProducts.ToArray(),
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);

            listOfProducts = GetProductsAssociatedTo(productGroupId.Value, outpostId.Value);

            return Json(new ProductsForChartOutputModel
            {
                Products = listOfProducts.ToArray(),
                TotalItems = listOfProducts.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<ProductsChartModel> GetProductsAssociatedTo(Guid productGroupId, Guid outpostId)
        {
            LoadUserAndClient();
            List<ProductsChartModel> listOfProducts = new List<ProductsChartModel>();

            var reportOutpostLevel = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();
            var reportOutpostLevelTreeModel = GetReportOutpostTreeModel(reportOutpostLevel);

            var outpost = reportOutpostLevelTreeModel.children.Find(it => it.Id == outpostId);
            var pg = outpost.children.Find(it => it.Id == productGroupId);
            foreach (var product in pg.children)
            {
                ProductsChartModel model = new ProductsChartModel() { ProductName = product.Name, StockLevel = product.ProductLevelSum };
                listOfProducts.Add(model);
            }

            return listOfProducts;
        }


        public JsonResult GetOutposts(Guid? CountryId, Guid? RegionId,Guid? DistrictId)
        {
            LoadUserAndClient();

            var outposts = new List<Outpost>();
            var outpostList = new List<EntityModel>();

            var modelForAllOption = new EntityModel();
            modelForAllOption.Id = ID_ALL_OPTION_FOR_OUTPOSTS;
            modelForAllOption.Name = "All";
            outpostList.Add(modelForAllOption);

            var outpostsQ = QueryOutpost.Query().Where(it => it.Client.Id == _client.Id);

            if (CountryId != null && CountryId.Value != Guid.Empty)
            {
                outpostsQ = outpostsQ.Where(it => it.Country.Id == CountryId.Value);
            }
            if (RegionId != null && RegionId.Value != Guid.Empty)
            {
                outpostsQ = outpostsQ.Where(it => it.Region.Id == RegionId.Value);
            }
            if (DistrictId != null && DistrictId.Value != Guid.Empty)
            {
                outpostsQ = outpostsQ.Where(it => it.District.Id == DistrictId.Value);
            }
            outposts = outpostsQ.ToList();
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

    }
}
