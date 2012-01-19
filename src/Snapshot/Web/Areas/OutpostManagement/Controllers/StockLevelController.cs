using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using FluentNHibernate.MappingModel;
using Web.Areas.OutpostManagement.Models.StockLevel;
using Web.Areas.StockAdministration.Models.ProductGroup;
using Web.Areas.StockAdministration.Models.Product;
using AutoMapper;
using Persistence.Queries.Countries;
using Web.Areas.OutpostManagement.Models.Client;


namespace Web.Areas.OutpostManagement.Controllers
{
    public class StockLevelController : Controller
    {
        //
        // GET: /StockAdministration/StockLevel/

        //public OutpostStockLevelOutput1Model StockLevelOutputModel { get; set; }

        public OutpostStockLevelInputModel StockLevelInputModel { get; set; }

        public OutpostStockLevelOutputModel OutpostStockLevelOverviewModel { get; set; }

        public ISaveOrUpdateCommand<OutpostStockLevel> SaveOrUpdateCommand { get; set; }

        public IQueryService<OutpostStockLevel> QueryStockLevel { get; set; }

        public IDeleteCommand<OutpostStockLevel> DeleteCommand { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Outpost> QueryOutposts { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public IQueryService<Product> QueryProduct { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";

        [HttpGet]
        public ActionResult OutpostStockLevelOverview( Guid outpostId, Guid? productGroupId)
        {
            var overviewModel = new OutpostStockLevelOutputModel();
            var productGroups = QueryProductGroup.Query();
            var stockLevels = QueryStockLevel.Query().Where(it => it.OutpostId == outpostId);

            overviewModel.ProductGroups = new List<SelectListItem>();
            foreach (var item in productGroups)
            {
                overviewModel.ProductGroups.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            var _outpost = QueryOutposts.Load(outpostId);
            overviewModel.OutpostId = _outpost.Id;
            overviewModel.OutpostName = _outpost.Name;
            if (productGroupId != null)
            {
                var productGroupId1 = new Guid(productGroupId.ToString());
                overviewModel.ProductGroupId = productGroupId1;
            }

            if (overviewModel.StockLevels == null)
                overviewModel.StockLevels = new List<OutpostStockLevelModel>();

            if (overviewModel.Products == null)
                overviewModel.Products = new List<SelectedProductlModel>();

            if (overviewModel.StockLevels == null)
                overviewModel.StockLevels = new List<OutpostStockLevelModel>();


            stockLevels.ToList().ForEach(stockLevelItem => overviewModel
                .StockLevels
                .Add(new OutpostStockLevelModel
                {
                    OutpostId = overviewModel.OutpostId,
                    ProdGroupId = overviewModel.ProductGroupId,
                    ProductGroupName = stockLevelItem.ProductGroupName,
                    ProductName = stockLevelItem.ProductName,
                    ProductId = stockLevelItem.ProductId,
                    ProdSmsRef = stockLevelItem.ProdSmsRef,
                    StockLevel = stockLevelItem.StockLevel,
                    PrevStockLevel = stockLevelItem.PrevStockLevel,
                    Updated = DateTime.Now

                }));

            return View(overviewModel);
        }

        public PartialViewResult OverviewProducts(Guid outpostId, Guid productGroupId)
                                                  
        {
            var overviewModel = new OutpostStockLevelOutputModel();
            var products = QueryProduct.Query().Where(p=>p.ProductGroup.Id == productGroupId);
            var productGroup = QueryProductGroup.Load(productGroupId);
            var stockLevels = QueryStockLevel.Query().Where(it => it.OutpostId == outpostId);
          

            var _outpost = QueryOutposts.Load(outpostId);

            overviewModel.OutpostId = _outpost.Id;
            overviewModel.ProductGroupId = productGroupId;
            overviewModel.ProductGroupName = productGroup.Name;

            var productsList = QueryProduct.Query().Where(m => m.ProductGroup.Id == overviewModel.ProductGroupId);
            var stockLevelList = QueryStockLevel.Query().Where(m => m.ProdGroupId == overviewModel.ProductGroupId && m.OutpostId == overviewModel.OutpostId);


            if (overviewModel.Products == null)
            {
                overviewModel.Products = new List<SelectedProductlModel>();

                foreach (var prditem in productsList)
                {
                    var slitem = stockLevelList.Where(m => m.ProductId == prditem.Id);

                    if (slitem.Count() == 0)
                    {
                        overviewModel.Products.Add(new SelectedProductlModel
                                                {
                                                    ProductId = prditem.Id,
                                                    ProductGroupName = overviewModel.ProductGroupName,
                                                    ProdSmsRef = prditem.SMSReferenceCode,
                                                    ProductName = prditem.Name,
                                                    Updated = DateTime.Now,
                                                    Selected = false
                                                });
                    }
                    else
                    {
                        overviewModel.Products.Add(new SelectedProductlModel
                        {
                            ProductId = prditem.Id,
                            ProductGroupName = overviewModel.ProductGroupName,
                            ProdSmsRef = prditem.SMSReferenceCode,
                            ProductName = prditem.Name,
                            Updated = DateTime.Now,
                            Selected = true
                        });
                    }
                }
            }

            if (overviewModel.StockLevels == null)
                overviewModel.StockLevels = new List<OutpostStockLevelModel>();

 
            stockLevels.ToList().ForEach(stockLevelItem => overviewModel
                .StockLevels
                .Add(new OutpostStockLevelModel
                {
                    OutpostId = overviewModel.OutpostId,
                    ProdGroupId = overviewModel.ProductGroupId,
                    ProductGroupName = stockLevelItem.ProductGroupName,
                    ProductName = stockLevelItem.ProductName,
                    ProductId = stockLevelItem.ProductId,
                    ProdSmsRef = stockLevelItem.ProdSmsRef,
                    StockLevel = stockLevelItem.StockLevel,
                    PrevStockLevel = stockLevelItem.PrevStockLevel,
                    Updated = DateTime.Now

                }));
            //products.ToList().ForEach(prditem => overviewModel
                //    .Products.Add(new SelectedProductlModel
                //    {
                //        ProductId = prditem.Id,
                //        ProductGroupName = overviewModel.ProductGroupName,
                //        ProdSmsRef = prditem.SMSReferenceCode,
                //        ProductName = prditem.Name,
                //        Updated = DateTime.Now,
                //        Selected = new bool()
                //    }));

            return PartialView(overviewModel);
        }


        public PartialViewResult OverviewStockLevel(Guid outpostId, Guid productGroupId)
        {
            var overviewModel = new OutpostStockLevelOutputModel();
            var stockLevels = QueryStockLevel.Query().Where(it => it.OutpostId == outpostId);
            var _outpost = QueryOutposts.Load(outpostId);

            overviewModel.OutpostId = _outpost.Id;
            overviewModel.ProductGroupId = productGroupId;
            //overviewModel.ProductGroupName = productGroup.Name;

            if (overviewModel.StockLevels == null)
                overviewModel.StockLevels = new List<OutpostStockLevelModel>();

            stockLevels.ToList().ForEach(stockLevelItem => overviewModel
                .StockLevels
                .Add(new OutpostStockLevelModel
            {
                OutpostId = overviewModel.OutpostId,
                ProdGroupId = overviewModel.ProductGroupId,
                ProductGroupName = stockLevelItem.ProductGroupName,
                ProductName = stockLevelItem.ProductName,
                ProductId = stockLevelItem.ProductId,
                ProdSmsRef = stockLevelItem.ProdSmsRef,
                StockLevel = stockLevelItem.StockLevel,
                PrevStockLevel = stockLevelItem.PrevStockLevel,
                Updated = DateTime.Now

            }));
            
            return PartialView(overviewModel);
        }


        [HttpPost]
        public ActionResult OutpostStockLevelOverview(OutpostStockLevelOutputModel overviewModel)
        {

            return RedirectToAction("OutpostStockLevelOverview", "StockLevel", new { outpostId = overviewModel.OutpostId });
        }

        [HttpPost]
        public ActionResult CreateOutpostStockLevel(OutpostStockLevelInputModel outpostStockLevelInputModel, Guid outpostId)
        {
            var overviewModel = outpostStockLevelInputModel;
            var productGroup = QueryProductGroup.Load(overviewModel.ProductGroupId);
            overviewModel.ProductGroupName = productGroup.Name;
            var stockLevels = QueryStockLevel.Query().Where(it => it.OutpostId == outpostId);



            var stockLevel = new OutpostStockLevel();
            var client = QueryClients.Load(Client.DEFAULT_ID);


            foreach (var item in overviewModel.Products)
            {
                if (item.Selected)
                {
                    var product = QueryProduct.Load(item.ProductId);
                    stockLevel.OutpostId = overviewModel.OutpostId;
                    stockLevel.ProductGroupName = overviewModel.ProductGroupName;
                    stockLevel.ProdGroupId = overviewModel.ProductGroupId;
                    stockLevel.ProductId = item.ProductId;
                    stockLevel.ProductName = product.Name;
                    stockLevel.ProdSmsRef = product.SMSReferenceCode;
                    stockLevel.StockLevel = 0;
                    stockLevel.PrevStockLevel = 0;
                    stockLevel.UpdatedMethod = "SMS";
                    stockLevel.Client = client;

                    var stockLevelCheck = stockLevels.Where(m => m.ProductId == item.ProductId);

                    if (stockLevelCheck.Count() == 0)
                    {
                        SaveOrUpdateCommand.Execute(stockLevel);
                    }
                }
            }

            return RedirectToAction("OutpostStockLevelOverview", "StockLevel", new { outpostId = overviewModel.OutpostId, productGroupId = overviewModel.ProductGroupId });
        }
 


    }
}
