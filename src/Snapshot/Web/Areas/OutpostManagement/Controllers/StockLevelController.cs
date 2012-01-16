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

        public OutpostStockLevelOutputModel StockLevelOutputModel { get; set; }

        public OutpostStockLevelInputModel StockLevelInputModel { get; set; }

        public OutpostStockLevelOverviewModel OutpostStockLevelOverviewModel { get; set; }

        public ISaveOrUpdateCommand<OutpostStockLevel> SaveOrUpdateCommand { get; set; }

        public IQueryService<OutpostStockLevel> QueryStockLevel { get; set; }

        public IDeleteCommand<OutpostStockLevel> DeleteCommand { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Outpost> QueryOutposts { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public IQueryService<Product> QueryProduct { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";

        [HttpGet]
        public ActionResult OutpostStockLevelOverview([Bind(Include="Products, StockLevels, ProductGroups, OutpostId, ProdGroupId")] Guid outpostId)
        {
            var overviewModel = new OutpostStockLevelOverviewModel();
            var productGroups = QueryProductGroup.Query();
            CreateMappings();

            overviewModel.ProductGroups = new List<SelectListItem>();
            foreach (var item in productGroups)
            {
                overviewModel.ProductGroups.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            var _outpost = QueryOutposts.Load(outpostId);
            overviewModel.OutpostId = _outpost.Id;
            overviewModel.OutpostName = _outpost.Name;
            if (overviewModel.StockLevels == null)
                overviewModel.StockLevels = new List<OutpostStockLevelModel>();

            if (overviewModel.Products == null)
                overviewModel.Products = new List<OutpostStockLevelModel>();

            return View(overviewModel);
        }

        public PartialViewResult OverviewProducts([Bind(Include = "Products, StockLevels, ProductGroups, OutpostId, ProdGroupId")] Guid productGroupId)
                                                  
        {
            //outpostStockLevelOverviewModel
            var productsList = new List<OutpostStockLevelModel>();
            var products = QueryProduct.Query().Where(m => m.ProductGroup.Id == productGroupId);

            foreach (var item in products)
            {
                var outpostStockLevelModel = new OutpostStockLevelModel();
                var outpostStockLevel = new OutpostStockLevel();
                //outpostStockLevel.OutpostId = OutpostId;
                outpostStockLevel.ProdGroupId = productGroupId;
                outpostStockLevel.ProductId = item.Id;
                outpostStockLevel.ProdSmsRef = item.SMSReferenceCode;
                outpostStockLevel.ProductName = item.Name;
                Mapper.Map(outpostStockLevel, outpostStockLevelModel);
                productsList.Add(outpostStockLevelModel);
                //outpostStockLevelOverviewModel.Products.Add(outpostStockLevelModel);
                
            }
            return PartialView(productsList);
        }


        public PartialViewResult OverviewStockLevel([Bind(Include = "Products, StockLevels, ProductGroups, OutpostId, ProdGroupId")] Guid productGroupId)
        {
            var stockLevel = QueryStockLevel.Query().Where(it => it.ProdGroupId == productGroupId);

            var stockLevelList = new List<OutpostStockLevelModel>();
            foreach (var item in stockLevel)
            {
                var outpostStockLevelModel = new OutpostStockLevelModel();

                Mapper.Map(item, outpostStockLevelModel);
                stockLevelList.Add(outpostStockLevelModel);
                //OutpostStockLevelOverviewModel.StockLevels.Add(outpostStockLevelModel);
            }
            
            return PartialView(stockLevelList);
        }


        [HttpPost]
        public ActionResult OutpostStockLevelOverview([Bind(Include = "Products, StockLevels, ProductGroups, OutpostId, ProdGroupId")] OutpostStockLevelOverviewModel overviewModel)
        {
            var productGroups = QueryProductGroup.Query();
            CreateMappings();
            overviewModel.ProductGroups = new List<SelectListItem>();
            foreach (var item in productGroups)
            {
                overviewModel.ProductGroups.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            UpdateModel(overviewModel);
            return View("OutpostStockLevelOverview", overviewModel);
        }

        [HttpPost]
        public ActionResult CreateOutpostStockLevel(OutpostStockLevelOverviewModel outpostStockLevelOverviewModel)
        {
            var overviewModel = outpostStockLevelOverviewModel;//var _outpost = QueryOutposts.Query().Where(m => m.Name == outpostStockLevelOverviewModel.OutpostName);
            //OutpostStockLevelOverviewModel outpostStockLevelOverviewModel1 = (OutpostStockLevelOverviewModel)DependencyResolver.Current.GetService(typeof(OutpostStockLevelOverviewModel));
            var productGroups = QueryProductGroup.Query();
            overviewModel.ProductGroups = new List<SelectListItem>();

            overviewModel.ProductGroups = new List<SelectListItem>();
            foreach (var item in productGroups)
            {
                overviewModel.ProductGroups.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return View("OutpostStockLevelOverview", outpostStockLevelOverviewModel);
       }
        //
        // GET: /StockAdministration/StockLevel/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /StockAdministration/StockLevel/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /StockAdministration/StockLevel/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /StockAdministration/StockLevel/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Overview");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /StockAdministration/StockLevel/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /StockAdministration/StockLevel/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Overview");
            }
            catch
            {
                return View();
            }
        }

        private static void CreateMappings(OutpostStockLevel entity = null)
        {
            Mapper.CreateMap<OutpostStockLevelModel, OutpostStockLevel>();
            Mapper.CreateMap<OutpostStockLevel, OutpostStockLevelModel>();

            Mapper.CreateMap<OutpostStockLevel, OutpostStockLevelInputModel>();
            Mapper.CreateMap<OutpostStockLevel, OutpostStockLevelOutputModel>();

            Mapper.CreateMap<OutpostStockLevelInputModel, OutpostStockLevel>();
            Mapper.CreateMap<OutpostStockLevelOutputModel, OutpostStockLevel>();

            Mapper.CreateMap<ClientModel, Client>();
            Mapper.CreateMap<Client, ClientModel>();
       
        }

    }
}
