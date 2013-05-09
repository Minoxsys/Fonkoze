using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Domain;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.StockAdministration.Models.HistoricalProductLevel;
using Web.Helpers;
using Web.Models.Shared;
using Core.Security;
using Web.Security;
using Web.Models.UserManager;
using Web.Areas.AnalysisManagement.Models.ReportRegionLevel;

namespace Web.Areas.StockAdministration.Controllers
{
    public class HistoricalProductLevelController : Controller
    {

        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }
        public IQueryService<OutpostHistoricalStockLevel> QueryHistorical { get; set; }
        public IQueryService<OutpostStockLevel> QueryStockLevel { get; set; }
        public IQueryService<ProductSale> QueryProductSale { get; set; }

        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> SaveOrUpdateMethod { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IPermissionsService PermissionService { get; set; }

        private const String HISTORICALOUTPOSTSTOCKLEVEL_EDIT_PERMISSION = "HistoricalOutpostStockLevel.Edit";
        private Client _client;
        private User _user;

        const string GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = "00000000-0000-0000-0000-000000000000";

        [Requires(Permissions = "HistoricalOutpostStockLevel.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToEdit = (PermissionService.HasPermissionAssigned(HISTORICALOUTPOSTSTOCKLEVEL_EDIT_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
           

            return View();
        }

         [Requires(Permissions = "HistoricalOutpostStockLevel.View")]
        public ActionResult GraphicOverview()
        {
            ViewBag.HasNoRightsToEdit = (PermissionService.HasPermissionAssigned(HISTORICALOUTPOSTSTOCKLEVEL_EDIT_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();


            return View();
        }

        [HttpGet]
        public JsonResult GetHistoricalProductLevel(Guid? countryId, Guid? regionId, Guid? districtId, Guid? outpostId)
        {
            LoadUserAndClient();
            var outpostsList = GetListOfOutpostFor(countryId, regionId, districtId, outpostId);

            List<OutpostGridModel> historicalModelListProjection = GetHistoricalStockLevelsFor(outpostsList);
            int totalItems = historicalModelListProjection.Count();

            return Json(new OutpostIndexGridModel
            {
                Historical = historicalModelListProjection.ToArray(),
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        public List<Outpost> GetListOfOutpostFor(Guid? countryId, Guid? regionId, Guid? districtId, Guid? outpostId)
        {
            var outposts = QueryOutpost.Query()
                    .Where(it => it.Client.Id == this._client.Id);

            if (countryId.HasValue && countryId != Guid.Empty)
                outposts = outposts.Where(it => it.Country.Id == countryId);
            if (regionId.HasValue && regionId != Guid.Empty)
                outposts = outposts.Where(it => it.Region.Id == regionId);
            if (districtId.HasValue && districtId != Guid.Empty)
                outposts = outposts.Where(it => it.District.Id == districtId);
            if (outpostId.HasValue && outpostId.ToString() != GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST)
                outposts = outposts.Where(it => it.Id == outpostId);

            return outposts.ToList();
        }

        private List<OutpostGridModel> GetHistoricalStockLevelsFor(List<Outpost> outpostsList)
        {
            List<OutpostGridModel> historicalGridList = new List<OutpostGridModel>();
            foreach (var outpost in outpostsList)
            {
                var historicalList = QueryHistorical.Query().Where(it => it.OutpostId == outpost.Id).ToList();

                foreach (var historical in historicalList)
                {
                    if (historical != null)
                    {
                        OutpostGridModel model = new OutpostGridModel();
                        model.Id = outpost.Id;
                        model.Name = outpost.Name;
                        var productGroup = QueryProductGroup.Load(historical.ProductGroupId);
                        model.ProductGroupId = historical.ProductGroupId;
                        model.ProductGroupName = historical.ProductGroupName;
                        model.SMSResponseDate = historical.UpdateDate.Value.ToString("dd-MMM-yyyy hh:mm tt");
                        model.NumberOfProducts = GetNumberOfProductsFor(outpost.Id, productGroup.Id, historical.UpdateDate.Value);

                        if (ExistsIn(historicalGridList, model) == false)
                            historicalGridList.Add(model);
                    }
                }
            }
            return historicalGridList;
        }

        private bool ExistsIn(List<OutpostGridModel> historicalGridList, OutpostGridModel model)
        {
            return historicalGridList.Exists(it => it.Name == model.Name &&
                                                   it.ProductGroupId == model.ProductGroupId &&
                                                   it.ProductGroupName == model.ProductGroupName &&
                                                   it.SMSResponseDate == model.SMSResponseDate &&
                                                   it.NumberOfProducts == model.NumberOfProducts);
        }

        private int GetNumberOfProductsFor(Guid outpostId, Guid productGroupId, DateTime dateTime)
        {
            return QueryHistorical.Query()
                .Where(it => it.OutpostId == outpostId)
                .Where(it => it.ProductGroupId == productGroupId)
                .Where(it => it.UpdateDate.Value.Year == dateTime.Year)
                .Where(it => it.UpdateDate.Value.Month == dateTime.Month)
                .Where(it => it.UpdateDate.Value.Day == dateTime.Day).Count();
        }

        


        [HttpGet]
        public JsonResult GetProductSales(Guid? countryId, Guid? regionId, Guid? districtId, Guid? outpostId, DateTime? startDate, DateTime? endDate, Guid? productId, string clientId)
        {

            var ps = QueryProductSale.Query();                  

            if (countryId.HasValue && countryId != Guid.Empty)
                ps = ps.Where(it => it.Outpost.Country.Id == countryId);
            if (regionId.HasValue && regionId != Guid.Empty)
                ps = ps.Where(it => it.Outpost.Region.Id == regionId);
            if (districtId.HasValue && districtId != Guid.Empty)
                ps = ps.Where(it => it.Outpost.District.Id == districtId);
            if (outpostId.HasValue && outpostId.ToString() != GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST)
                ps = ps.Where(it => it.Outpost.Id == outpostId);
            if (startDate.HasValue)
                ps = ps.Where(it => it.Created.Value.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                ps = ps.Where(it => it.Created.Value.Date <= endDate.Value.Date);
            if (productId.HasValue && productId != Guid.Empty)
                ps = ps.Where(it => it.Product.Id == productId);
            if (clientId!=null)
            {
                if (clientId != "0")
                {
                    if (clientId == "F")
                        ps = ps.Where(it => (it.ClientIdentifier == "F" || it.ClientIdentifier == "f"));
                    else
                        ps = ps.Where(it => (it.ClientIdentifier == "N" || it.ClientIdentifier == "n"));
                }
            }

            var psms = new List<ProductSaleModel>();
            foreach (var productSale in ps.ToList())
            {
                var psm = new ProductSaleModel() { OutpostName = productSale.Outpost.Name, ProductName = productSale.Product.Name, Date = productSale.Created.ToString(), Quantity = productSale.Quantity };
                psms.Add(psm);
            }
          
            return Json(new ProductsSaleOutputModel
            {
                ProductSales = psms.ToArray(),
                TotalItems = 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductGroupLevels(Guid? outpostId, Guid? productGroupId, DateTime? smsResponseDate)
        {
            if (outpostId.HasValue && productGroupId.HasValue && smsResponseDate.HasValue)
            {

                var queryHistorical = QueryHistorical.Query()
                    .Where(it => it.OutpostId == outpostId)
                    .Where(it => it.ProductGroupId == productGroupId)
                    .Where(it => it.UpdateDate.Value.Year == smsResponseDate.Value.Year)
                    .Where(it => it.UpdateDate.Value.Month == smsResponseDate.Value.Month)
                    .Where(it => it.UpdateDate.Value.Day == smsResponseDate.Value.Day);

                var productGroupLevels = new List<ProductGroupLevelModel>();
                foreach (var productGroupLevel in queryHistorical.ToList())
                {
                    var level = new ProductGroupLevelModel();
                    level.Id = productGroupLevel.Id;
                    level.OutpostId = productGroupLevel.OutpostId;
                    level.OutpostName = productGroupLevel.OutpostName;
                    level.ProductGroupId = productGroupLevel.ProductGroupId;
                    level.ProductGroupName = productGroupLevel.ProductGroupName;
                    level.ProductId = productGroupLevel.ProductId;
                    level.ProductName = productGroupLevel.ProductName;
                    level.ProductStockLevel = productGroupLevel.StockLevel;
                    level.SMSReferenceCode = productGroupLevel.ProdSmsRef;
                    level.LastUpdated = DateFormatter.DateToShortString(productGroupLevel.UpdateDate.Value);
                    level.Description = QueryProductGroup.Load(productGroupId.Value).Description;

                    productGroupLevels.Add(level);
                }

                return Json(new ProductsIndexOutputModel
                {
                    Products = productGroupLevels.ToArray(),
                    TotalItems = productGroupLevels.Count()
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new ProductsIndexOutputModel
            {
                Products = null,
                TotalItems = 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(HistoricalInputModel model)
        {
            if (model.Id == Guid.Empty)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "You must supply a historicalId in order to edit the historical stock level."
                   });
            }

            var historical = QueryHistorical.Load(model.Id);
            if (historical.StockLevel != model.StockLevel)
            {
                historical.PrevStockLevel = historical.StockLevel;
                historical.StockLevel = model.StockLevel;

                SaveOrUpdateMethod.Execute(historical);
            }

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = "The Historical stock level has been saved."
               });
        }

        [HttpGet]
        public JsonResult GetProducts(Guid? countryId, Guid? regionId, Guid? districtId, Guid? outpostId)
        {
            LoadUserAndClient();
            var prodsList = new List<ReferenceModel>();
            prodsList.Add(new ReferenceModel() { Id = Guid.Empty, Name = "All" });

            if (!countryId.HasValue && !regionId.HasValue && !districtId.HasValue && !outpostId.HasValue)
            {
                return Json(new ProductsReferenceOutputModel
                {
                    Products = prodsList.ToArray(),
                    TotalItems = prodsList.Count()
                }, JsonRequestBehavior.AllowGet);

            }


            var osls = QueryStockLevel.Query().Where(it => it.Client.Id == this._client.Id);
            if (countryId.HasValue && countryId != Guid.Empty)
            {
                osls = osls.Where(it => it.Outpost.Country.Id == countryId);
            }
            if (regionId.HasValue && regionId != Guid.Empty)
            {
                osls = osls.Where(it => it.Outpost.Region.Id == regionId);
            }
            if (districtId.HasValue && districtId != Guid.Empty)
            {
                osls = osls.Where(it => it.Outpost.District.Id == districtId);
            }
            if (outpostId.HasValue && outpostId != Guid.Empty)
            {
                osls = osls.Where(it => it.Outpost.Id == outpostId);
            }
            IQueryable<Product> prodsDistinct = (from p in osls select p.Product).Distinct();//.OrderBy(p=>p.Name);
            
            foreach (var p in prodsDistinct.ToList())
            {
               prodsList.Add(new ReferenceModel() { Id = p.Id, Name = p.Name });
            }

            return Json(new ProductsReferenceOutputModel
            {
                Products = prodsList.ToArray(),
                TotalItems = prodsList.Count()
            }, JsonRequestBehavior.AllowGet);


        }

        

        [HttpGet]
        public JsonResult GetOutposts(Guid? districtId)
        {
            LoadUserAndClient();

            var outpostModelListProjection = new List<GetOutpostsOutputModel.OutpostModel>();
            outpostModelListProjection.Add(new GetOutpostsOutputModel.OutpostModel() { Id = GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST, Name = " All" });

            if (!districtId.HasValue)
                return Json(new GetOutpostsOutputModel
                {
                    Outposts = outpostModelListProjection.ToArray(),
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);

            var outposts = QueryOutpost.Query().Where(it => it.Client.Id == this._client.Id);

            if (districtId.HasValue)
                outposts = outposts.Where(it => it.District.Id == districtId);

            int totalItems = outposts.Count();

            foreach (var outpost in outposts.ToList())
            {
                var model = new GetOutpostsOutputModel.OutpostModel();
                model.Id = outpost.Id.ToString();
                model.Name = outpost.Name;
                outpostModelListProjection.Add(model);
            }

            return Json(new GetOutpostsOutputModel
            {
                Outposts = outpostModelListProjection.ToArray(),
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }



    }
}
