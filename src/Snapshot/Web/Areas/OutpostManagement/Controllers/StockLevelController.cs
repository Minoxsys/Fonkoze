using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
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

        public ISaveOrUpdateCommand<OutpostStockLevel> SaveOrUpdateCommand { get; set; }

        public IQueryService<OutpostStockLevel> QueryService { get; set; }

        public IDeleteCommand<OutpostStockLevel> DeleteCommand { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Outpost> QueryOutposts { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public IQueryService<Product> QueryProduct { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";

        [HttpGet]
        public ActionResult OutpostStockLevelOverview(Guid outpostId)
        {
            OutpostStockLevelOverviewModel overviewModel = new OutpostStockLevelOverviewModel();

            var _outpost = QueryOutposts.Load(outpostId);
            overviewModel.ProductGroups = new List<SelectListItem>();
            overviewModel.OutpostName = _outpost.Name;

            List<SelectListItem> productGroups = new List<SelectListItem>();

            var productGroupItems = QueryProductGroup.Query();

            foreach(ProductGroup productGroup in productGroupItems)
            {
                overviewModel.ProductGroups.Add(new SelectListItem { Text = productGroup.Name, Value = productGroup.Id.ToString() });
            }

            return View(overviewModel);
        }
 
        public PartialViewResult OverviewProducts(Guid productGroupId)
        {
            var productList = new List<Product>();

            var productGroup = QueryService.Load(productGroupId);
            
            //foreach (var item in productGroup.Products)
            //{

            //    CreateMappings();
            //    var productModel = new Product();
            //    Mapper.Map(item, productModel);
            //    productList.Add(productModel);

            //}
            return PartialView(productList);
        }

        public ActionResult Details(int id)
        {
            return View();
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

        private void CreateMappings()
        {
            Mapper.CreateMap<ProductModel, Product>();
            Mapper.CreateMap<Product, ProductModel>();

            Mapper.CreateMap<ProductInputModel.ProductGroupInputModel, ProductGroup>();
            Mapper.CreateMap<ProductGroup, ProductInputModel.ProductGroupInputModel>();

            Mapper.CreateMap<ProductInputModel, Product>();
            Mapper.CreateMap<Product, ProductInputModel>();

            Mapper.CreateMap<Product, ProductOutputModel>();
            Mapper.CreateMap<ProductOutputModel, Product>();


            Mapper.CreateMap<Client, ClientModel>();
            Mapper.CreateMap<ClientModel, Client>();
        }

    }
}
