using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Persistence.Queries.Products;
using Web.Areas.StockAdministration.Models.Product;
using Web.Areas.StockAdministration.Models.ProductGroup;
using AutoMapper;


namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductGroupController : Controller
    {
        //
        //public IQueryProducts QueryProductGroups { get; set; }
        public IQueryService<ProductGroup> QueryService { get; set; }
        public ProductGroupOutputModel ProductGroupOutputModel { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }
        public ISaveOrUpdateCommand<ProductGroup> SaveOrUpdateProductGroup { get; set; }
        public IDeleteCommand<ProductGroup> DeleteCommand { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }

        public ActionResult Overview()
        {
            var overviewModel = new ProductGroupOverviewModel();

            var productGroups = QueryService.Query();

            if (productGroups.ToList().Count > 0)
            {
                productGroups.ToList().ForEach(item =>
                                                   {
                                                       CreateMappings();
                                                       var productGroupModel = new ProductGroupModel();
                                                       Mapper.Map(item, productGroupModel);
                                                       overviewModel.ProductGroups.Add(productGroupModel);

                                                   });
            }
            //overviewModel.ProductGroups.Add(null);
            return View(overviewModel);
        }

        private void CreateMappings()
        {
            Mapper.CreateMap<ProductGroupModel, ProductGroup>();
            Mapper.CreateMap<ProductGroup, ProductGroupModel>();

            Mapper.CreateMap<ProductGroupInputModel, ProductGroup>();
            Mapper.CreateMap<ProductGroupOutputModel, ProductGroupInputModel>();

            // Mapper.CreateMap<ProductsInputModel, Products>();
            //Mapper.CreateMap<Products, ProductsInputModel>();

            Mapper.CreateMap<ProductGroup, ProductGroupOutputModel>();
            Mapper.CreateMap<ProductGroupOutputModel, ProductGroup>();

            //Mapper.CreateMap<ProductOutputModel.OutpostModel, Outpost>();
            //Mapper.CreateMap<Outpost, ProductsOutputModel.OutpostModel>();
        }

        public ViewResult Create()
        {
            //var ProductGroupOutputModel = new ProductGroupOutputModel();
            return View(ProductGroupOutputModel);
        }

        [HttpPost]
        public ActionResult Create(ProductGroupInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var ProductsOutputModel = BuildProductsOutputModelFromInputModel(model);

                return View("Create", ProductsOutputModel);
            }

            CreateMappings();

            var ProductGroup = new ProductGroup();
            Mapper.Map(model, ProductGroup);



            SaveOrUpdateProductGroup.Execute(ProductGroup);

            return RedirectToAction("Overview");
        }

        private ProductGroupOutputModel BuildProductsOutputModelFromInputModel(ProductGroupInputModel model)
        {
            var ProductsOutputModel = new ProductGroupOutputModel();
            ProductsOutputModel.Description = model.Description;
            ProductsOutputModel.Id = model.Id;
            ProductsOutputModel.Name = model.Name;
            return ProductsOutputModel;
        }

        public ViewResult Edit(Guid guid)
        {
            var productGroupOutputModel = new ProductGroupOutputModel();

            var productGroup = QueryService.Load(guid);

            CreateMappings();

            Mapper.Map(productGroup, productGroupOutputModel);

            return View(productGroupOutputModel);
        }

        [HttpPost]
        public ActionResult Edit(ProductGroupInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var ProductGroupOutputModel = BuildProductsOutputModelFromInputModel(model);
                return View("Edit", ProductGroupOutputModel);
            }

            CreateMappings();

            var ProductGroup = new ProductGroup();

            Mapper.Map(model, ProductGroup);

            SaveOrUpdateProductGroup.Execute(ProductGroup);

            return RedirectToAction("Overview");
        }

        public ViewResult EditProducts(Guid guid)
        {
            var ProductGroupOutputModel = new ProductGroupOutputModel();

            var ProductGroup = QueryService.Load(guid);

            CreateMappings();

            Mapper.Map(ProductGroup, ProductGroupOutputModel);

            return View(ProductGroupOutputModel);
        }

        [HttpPost]
        public ActionResult EditProducts(ProductGroupInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var ProductGroupOutputModel = BuildProductsOutputModelFromInputModel(model);
                return View("Edit", ProductGroupOutputModel);
            }

            CreateMappings();

            var ProductGroup = new ProductGroup();

            Mapper.Map(model, ProductGroup);

            SaveOrUpdateProductGroup.Execute(ProductGroup);

            return RedirectToAction("Overview");
        }

        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid productGroupId)
        {
            var productGroup = QueryService.Load(productGroupId);
            //var productResults = QueryProduct.Query();

            //if (productResults != null)
            //{
                if (productGroup != null)
                {
                    //productResults = QueryProduct.Query().Where(it => it.ProductGroup.Id == productGroup.Id);

                    //    if (productResults.ToList().Count != 0)
                    //    {
                    //        TempData.Add("error", string.Format("The Product Group {0} has regions associated, so it can not be deleted", country.Name));
                    //        return RedirectToAction("Overview", new { page = 1 });
                    //    }
                }

                DeleteCommand.Execute(productGroup);
            //}

            return RedirectToAction("Overview");
        }
    }
}
