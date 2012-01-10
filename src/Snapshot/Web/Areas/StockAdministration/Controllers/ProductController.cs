using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Persistence.Queries.Products;
using Web.Areas.StockAdministration.Models.Product;
using AutoMapper;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductController : Controller
    {
        public IQueryProduct QueryProduct { get; set; }

        public IQueryService<Product> QueryService { get; set; }

        public ProductOutputModel ProductOutputModel { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public ISaveOrUpdateCommand<Product> SaveOrUpdateProduct { get; set; }

        public IDeleteCommand<Product> DeleteProduct { get; set; }

        public IQueryService<Outpost> QueryOutposts { get; set; }

        public ActionResult Overview()
        {
            var overviewModel = new ProductOverviewModel();

            var products = QueryProduct.GetAll();

            if (products.ToList().Count > 0)
            {
                foreach (Product product in products)
                {
                    CreateMappings();
                    var productModel = new ProductModel();
                    Mapper.Map(product, productModel);
                    overviewModel.Products.Add(productModel);
                    
                }
            }

            return View(overviewModel);
        }

        private void CreateMappings()
        {
            Mapper.CreateMap<ProductModel, Product>();
            Mapper.CreateMap<Product, ProductModel>();

            Mapper.CreateMap<ProductGroupModel, ProductGroup>();
            Mapper.CreateMap<ProductGroup, ProductGroupModel>();

            Mapper.CreateMap<ProductInputModel.ProductGroupInputModel, ProductGroup>();
            Mapper.CreateMap<ProductGroup, ProductInputModel.ProductGroupInputModel>();

            Mapper.CreateMap<ProductInputModel, Product>();
            Mapper.CreateMap<Product, ProductInputModel>();

            Mapper.CreateMap<Product, ProductOutputModel>();
            Mapper.CreateMap<ProductOutputModel, Product>();

            Mapper.CreateMap<OutpostModel, Outpost>();
            Mapper.CreateMap<Outpost, OutpostModel>();

            Mapper.CreateMap<Region, RegionModel>();
            Mapper.CreateMap<RegionModel, Region>();

            Mapper.CreateMap<District,DistrictModel>();
            Mapper.CreateMap<DistrictModel,District>();

            Mapper.CreateMap<Client, ClientModel>();
            Mapper.CreateMap<ClientModel,Client>();
        }

        public ViewResult Create()
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup, QueryOutposts);
            return View(productOutputModel);
        }
        public ViewResult CreateProduct(Guid? ProductGroupId)
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup, QueryOutposts);

            if (ProductGroupId != null)
            {
                if(productOutputModel.ProductGroups.Where(it=>it.Value == ProductGroupId.Value.ToString()).ToList().Count > 0)
                {
                    productOutputModel.ProductGroups.First(it => it.Value == ProductGroupId.Value.ToString()).Selected = true;
                }
            }
            return View("Create",productOutputModel);
        }
        [HttpPost]
        public ActionResult Create(ProductInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var productOutputModel = BuildProductOutputModelFromInputModel(model);

                return View("Create", productOutputModel);
            }

            CreateMappings();

            var product = new Product();
            Mapper.Map(model, product);

            
            product.ProductGroup = QueryProductGroup.Load(model.ProductGroup.Id);
            
            SaveOrUpdateProduct.Execute(product);

            return RedirectToAction("Overview");
        }

        private ProductOutputModel BuildProductOutputModelFromInputModel(ProductInputModel model)
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup,QueryOutposts);
            productOutputModel.Description = model.Description;
            productOutputModel.Id = model.Id;
            productOutputModel.LowerLimit = model.LowerLimit;
            productOutputModel.UpperLimit = model.UpperLimit;
            productOutputModel.Name = model.Name;
            productOutputModel.SMSReferenceCode = model.SMSReferenceCode;
            productOutputModel.ProductGroup.Id = model.ProductGroup.Id;
            if (model.ProductGroup != null)
            {
                if (model.ProductGroup.Id != null)
                {
                    if (productOutputModel.ProductGroups.Where(it => it.Value == model.ProductGroup.Id.ToString()).ToList().Count > 0)
                        productOutputModel.ProductGroups.First(it => it.Value == model.ProductGroup.Id.ToString()).Selected = true;
                }
            }
          return productOutputModel;
        }

        public ViewResult Edit(Guid guid)
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup,QueryOutposts);

            var product = QueryService.Load(guid);

            CreateMappings();

            Mapper.Map(product, productOutputModel);

            if (productOutputModel.ProductGroups.Where(it => it.Value == product.ProductGroup.Id.ToString()).ToList().Count > 0)
            {
                productOutputModel.ProductGroups.First(it => it.Value == product.ProductGroup.Id.ToString()).Selected = true; 
            }

            productOutputModel.PreviousStockLevel = product.StockLevel;
            //productOutputModel.Outpost.Id = product.Outpost.Id;

            return View(productOutputModel);
        }

        [HttpPost]
        public ActionResult Edit(ProductInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var productOutputModel = BuildProductOutputModelFromInputModel(model);
                return View("Edit", productOutputModel);
            }

            CreateMappings();
            
            
            var product = new Product();

            Mapper.Map(model, product);

            product.ProductGroup = QueryProductGroup.Load(model.ProductGroup.Id);              
            

            SaveOrUpdateProduct.Execute(product);

            return RedirectToAction("Overview");
        }

        [HttpPost]
        public ActionResult Delete(Guid guid)
        {
            var product = QueryService.Load(guid);

            DeleteProduct.Execute(product);

            return RedirectToAction("Overview");
        }
    }
}
