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

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class ReportRegionLevelController : Controller
    {
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        private Client _client;
        private User _user;

        private const string NAME_ALL_OPTION = "All";
        private Guid ID_ALL_OPTION_FOR_COUNTRIES = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private Guid ID_ALL_OPTION_FOR_REGIONS = Guid.Parse("00000000-0000-0000-0000-000000000002");

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

            countryList.Add(new LocationEntityModel { Name = NAME_ALL_OPTION, Id = ID_ALL_OPTION_FOR_COUNTRIES });

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

            regionList.Add(new LocationEntityModel { Name = NAME_ALL_OPTION, Id = ID_ALL_OPTION_FOR_REGIONS });

            var regions = QueryRegion.Query().Where(it => it.Client.Id == _client.Id);

            if (CountryId != ID_ALL_OPTION_FOR_COUNTRIES)
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
        public JsonResult GetReports(InputModel inputModel)
        {
            var reportRegionLevelTreeModel = new ReportRegionLevelTreeModel { Name = "root" };
            LoadUserAndClient();

            var reportRegionLevel = QueryOutpostStockLevel.Query().Where(it => it.Client.Id == _client.Id).ToList();

            if ((inputModel.RegionId != ID_ALL_OPTION_FOR_REGIONS) && (inputModel.RegionId != Guid.Empty))
            {
                reportRegionLevel = reportRegionLevel.Where(it => it.Outpost.Region.Id == inputModel.RegionId).ToList();
            }
            else
            {
                if (inputModel.CountryId != ID_ALL_OPTION_FOR_COUNTRIES)
                {
                    reportRegionLevel = reportRegionLevel.Where(it => it.Outpost.Country.Id == inputModel.CountryId).ToList();
                }
            }

            var reportRegionLevelGroupByRegion = reportRegionLevel.GroupBy(it => it.Outpost.Region);

            foreach (var regionGroup in reportRegionLevelGroupByRegion)
            {
                var regionNode = ToRegionNode(regionGroup);
                reportRegionLevelTreeModel.children.Add(regionNode);
            }

            return Json(reportRegionLevelTreeModel, JsonRequestBehavior.AllowGet);
        }

        private ReportRegionLevelTreeModel ToRegionNode(IGrouping<Region, OutpostStockLevel> regionGroup)
        {
            var regionNode = new ReportRegionLevelTreeModel { Name = regionGroup.Key.Name, ProductLevelSum = "" };
            regionNode.Name += " ( Quantity of Outposts:" + QueryOutpost.Query().Count(it => it.Region.Id == regionGroup.Key.Id) + " ) ";

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
            var productGroupNode = new ReportRegionLevelTreeModel { Name = productGroup.Key.Name, ProductLevelSum = "" };

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
            var leafNode = new ReportRegionLevelTreeModel { Name = item.Key.Name };
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
        public ActionResult Overview()
        {
            return View();
        }

    }
}
