using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Persistence.Queries.Products;
using Web.Areas.StockAdministration.Models.ProductGroup;
using AutoMapper;
using Web.Models.Shared;


namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductGroupController : Controller
    {
        //
        //public IQueryProducts QueryProductGroups { get; set; }
        public IQueryService<ProductGroup> QueryService { get; set; }
        public ProductGroupOutputModel ProductGroupOutputModel { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }
        public IQueryService<Product> QueryProducts { get; set; }
        public ISaveOrUpdateCommand<ProductGroup> SaveOrUpdateProductGroup { get; set; }
        public IDeleteCommand<ProductGroup> DeleteCommand { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }

        [HttpPost]
        public JsonResult Create(ProductGroupInputModel model)
        {
            if (string.IsNullOrEmpty(model.Name) && string.IsNullOrEmpty(model.Description))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The Product Group has not been saved!"
                   });
            }

            CreateMappings();

            var productGroup = new ProductGroup();
            Mapper.Map(model, productGroup);

            SaveOrUpdateProductGroup.Execute(productGroup);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Product Group {0} has been saved.", productGroup.Name)
               });
        }

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
                                                       productGroupModel.ProductsNo = QueryProducts.Query().Count<Product>(it => it.ProductGroup.Id == item.Id);
                                                       overviewModel.ProductGroups.Add(productGroupModel);

                                                   });
            }
            //overviewModel.ProductGroups.Add(null);
            if (TempData.ContainsKey("error"))
            {
                overviewModel.Error = (string)TempData["error"];
            }
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
          
        public ViewResult CreateProductGroupForProduct(bool CreateCommingFromProductCreate,bool CreateCommingFromProductEdit,Guid? productId)
        {
            var ProductGroupOutputModel = new ProductGroupOutputModel();
            TempData["FromProduct"] = CreateCommingFromProductCreate;
            TempData["FromProductEdit"] = CreateCommingFromProductEdit;
            if (productId != null)
            {
                TempData["ProductId"] = productId.Value.ToString();
            }
            return View("Create",ProductGroupOutputModel);
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
            var productResults = QueryProduct.Query();

            if (productResults != null)
            {
                if (productGroup != null)
                {
                    productResults = QueryProduct.Query().Where(it => it.ProductGroup.Id == productGroup.Id);

                        if (productResults.ToList().Count != 0)
                        {
                            TempData.Add("error", string.Format("The Product Group {0} has products associated, so it can not be deleted", productGroup.Name));
                            return RedirectToAction("Overview", new { page = 1 });
                        }
                }

                DeleteCommand.Execute(productGroup);
            }

            return RedirectToAction("Overview");
        }
    }
}
