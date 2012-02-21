using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.StockAdministration.Models.HistoricalProductLevel;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Models.Shared;
using Web.Helpers;

namespace Web.Areas.StockAdministration.Controllers
{
    public class HistoricalProductLevelController : Controller
    {

        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }
        public IQueryService<OutpostHistoricalStockLevel> QueryHistorical { get; set; }

        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> SaveOrUpdateMethod { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        const string GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = "00000000-0000-0000-0000-000000000023";


        public ActionResult Overview()
        {
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

            if (outpostId.HasValue && outpostId.ToString() != GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST)
                outposts = outposts.Where(it => it.Id == outpostId);
            else
                outposts = outposts.Where(it => it.District.Id == districtId);

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
                        model.ProductGroupName = productGroup.Name;
                        model.SMSResponseDate = DateFormatter.DateToShortString(historical.UpdateDate.Value);
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
                    level.OutpostId = outpostId.Value;
                    level.OutpostName = QueryOutpost.Load(outpostId.Value).Name;
                    level.ProductGroupId = productGroupId.Value;
                    level.ProductGroupName = QueryProductGroup.Load(productGroupId.Value).Name;
                    level.ProductId = productGroupLevel.ProductId;
                    level.ProductName = QueryProduct.Load(productGroupLevel.ProductId).Name;
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
        public JsonResult GetOutposts(Guid? districtId)
        {
            LoadUserAndClient();

            if (!districtId.HasValue)
                return Json(new GetOutpostsOutputModel
                {
                    Outposts = null,
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);

            var outposts = QueryOutpost.Query().Where(it => it.Client.Id == this._client.Id);

            if (districtId.HasValue)
                outposts = outposts.Where(it => it.District.Id == districtId);

            int totalItems = outposts.Count();

            var outpostModelListProjection = new List<GetOutpostsOutputModel.OutpostModel>();
            outpostModelListProjection.Add(new GetOutpostsOutputModel.OutpostModel() { Id = GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST, Name = "ALL" });

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
