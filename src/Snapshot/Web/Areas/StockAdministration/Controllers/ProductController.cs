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
using Web.Areas.OutpostManagement.Models;

namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductController : Controller
    {
        public IQueryService<Product> QueryService { get; set; }

        public ProductOutputModel ProductOutputModel { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public ISaveOrUpdateCommand<Product> SaveOrUpdateProduct { get; set; }

        public IDeleteCommand<Product> DeleteProduct { get; set; }

        public int PageSize = 8;

        public ActionResult Overview(Guid? productGroupId, int page)
        {
            var overviewModel = new ProductOverviewModel(QueryProductGroup);
            List<Product> products = new List<Product>();
            List<Product> paginatedProducts = new List<Product>();

            if (productGroupId != null)
            {
                products = QueryService.Query().Where(it => it.ProductGroup.Id == productGroupId.Value).ToList();
                paginatedProducts = QueryService.Query().Where(it => it.ProductGroup.Id == productGroupId.Value)
                             .OrderBy(p => p.Name)
                             .Skip((page - 1) * PageSize)
                             .Take(PageSize).ToList();

                if (overviewModel.ProductGroups.Where(it => it.Value == productGroupId.Value.ToString()).ToList().Count > 0)
                {
                    overviewModel.ProductGroups.First(it => it.Value == productGroupId.Value.ToString()).Selected = true;
                }

            }

            overviewModel.PartialViewModel = new PartialViewModel();
            overviewModel.PartialViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = products.Count()
            };
            if (paginatedProducts.ToList().Count > 0)
            {
                foreach (Product product in paginatedProducts)
                {
                    CreateMappings();
                    var productModel = new ProductModel();
                    Mapper.Map(product, productModel);
                    overviewModel.Products.Add(productModel);

                }
            }
            overviewModel.PartialViewModel.Products = overviewModel.Products;
            return View(overviewModel);
        }
        public PartialViewResult OverviewTable(Guid? productGroupId)
        {
            var partialviewModel = new PartialViewModel();

            var productList = new List<ProductModel>();

            if (!productGroupId.HasValue)
                return PartialView(partialviewModel);

            var allProducts = QueryService.Query().Where<Product>(it => it.ProductGroup.Id == productGroupId).ToList();
            var products = QueryService.Query().Where<Product>(it => it.ProductGroup.Id == productGroupId)
                             .OrderBy(p => p.Name)
                             .Skip((1 - 1) * PageSize)
                             .Take(PageSize).ToList();

            partialviewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = 1,
                ItemsPerPage = PageSize,
                TotalItems = allProducts.Count()
            };

            foreach (Product item in products)
            {
                CreateMappings();
                var productModel = new ProductModel();
                Mapper.Map(item, productModel);
                partialviewModel.Products.Add(productModel);

            }
            return PartialView(partialviewModel);
        }
        public PartialViewResult SearchForProductWithName(string productName)
        {
            var partialviewModel = new PartialViewModel();

            var productList = new List<ProductModel>();
            //if (productName == null)
            //    return PartialView(productList);
            var allProducts = QueryService.Query().Where<Product>(it => it.Name.Contains(productName)).ToList();
            var products = QueryService.Query().Where<Product>(it => it.Name.Contains(productName))
                             .OrderBy(p => p.Name)
                             .Skip((1 - 1) * PageSize)
                             .Take(PageSize).ToList();

            partialviewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = 1,
                ItemsPerPage = PageSize,
                TotalItems = allProducts.Count()
            };
            foreach (Product item in products)
            {
                CreateMappings();
                var productModel = new ProductModel();
                Mapper.Map(item, productModel);
                partialviewModel.Products.Add(productModel);

            }
            return PartialView("OverviewTable", partialviewModel);
        }

        public PartialViewResult GetItemsForPage(int page, Guid productGroupId, string NameToSearchFor)
        {

            var partialViewModel = new PartialViewModel();
            var products = new List<Product>();
            var allproducts = new List<Product>();

            if ((productGroupId != null) && (NameToSearchFor != null))
            {
                allproducts = QueryService.Query().Where(it => it.ProductGroup.Id == productGroupId && it.Name.Contains(NameToSearchFor)).ToList(); ;
                products = QueryService.Query().Where(it => it.ProductGroup.Id == productGroupId && it.Name.Contains(NameToSearchFor))
                            .OrderBy(p => p.Name)
                             .Skip((page - 1) * PageSize)
                             .Take(PageSize).ToList();
 
            }
            else
            {
                if ((productGroupId == null) && (NameToSearchFor != null))
                {
                    allproducts = QueryService.Query().Where(it => it.Name.Contains(NameToSearchFor)).ToList(); ;
                    products = QueryService.Query().Where(it => it.Name.Contains(NameToSearchFor))
                            .OrderBy(p => p.Name)
                             .Skip((page - 1) * PageSize)
                             .Take(PageSize).ToList();

                }
                else
                {
                    allproducts = QueryService.Query().Where(it => it.ProductGroup.Id == productGroupId).ToList(); ;
                    products = QueryService.Query().Where(it => it.ProductGroup.Id == productGroupId)
                                .OrderBy(p => p.Name)
                                 .Skip((page - 1) * PageSize)
                                 .Take(PageSize).ToList();
 
                }
            }
            partialViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = allproducts.Count()
            };

            foreach (Product item in products)
            {
                CreateMappings();
                var productModel = new ProductModel();
                Mapper.Map(item, productModel);
                partialViewModel.Products.Add(productModel);

            }
            return PartialView("OverviewTable", partialViewModel);
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

            Mapper.CreateMap<District, DistrictModel>();
            Mapper.CreateMap<DistrictModel, District>();

            Mapper.CreateMap<Client, ClientModel>();
            Mapper.CreateMap<ClientModel, Client>();
        }

        public ViewResult Create()
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup);
            return View(productOutputModel);
        }
        public ViewResult CreateProduct(Guid? ProductGroupId)
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup);

            if (ProductGroupId != null)
            {
                if (productOutputModel.ProductGroups.Where(it => it.Value == ProductGroupId.Value.ToString()).ToList().Count > 0)
                {
                    productOutputModel.ProductGroups.First(it => it.Value == ProductGroupId.Value.ToString()).Selected = true;
                }
            }
            return View("Create", productOutputModel);
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

            return RedirectToAction("Overview", new { productGroupId = product.ProductGroup.Id, page = 1 });
        }
        private ProductOutputModel BuildProductOutputModelFromInputModel(ProductInputModel model)
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup);
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
            var productOutputModel = new ProductOutputModel(QueryProductGroup);

            var product = QueryService.Load(guid);

            CreateMappings();

            Mapper.Map(product, productOutputModel);

            if (productOutputModel.ProductGroups.Where(it => it.Value == product.ProductGroup.Id.ToString()).ToList().Count > 0)
            {
                productOutputModel.ProductGroups.First(it => it.Value == product.ProductGroup.Id.ToString()).Selected = true;
            }

            return View(productOutputModel);
        }
        public ViewResult EditProduct(Guid guid, Guid productGroupId)
        {
            var productOutputModel = new ProductOutputModel(QueryProductGroup);

            var product = QueryService.Load(guid);

            CreateMappings();

            Mapper.Map(product, productOutputModel);

            if (productOutputModel.ProductGroups.Where(it => it.Value == productGroupId.ToString()).ToList().Count > 0)
            {
                productOutputModel.ProductGroups.First(it => it.Value == productGroupId.ToString()).Selected = true;
            }

            return View("Edit", productOutputModel);
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

            return RedirectToAction("Overview", new { productGroupId = product.ProductGroup.Id, page = 1 });
        }

        [HttpPost]
        public ActionResult Delete(Guid guid)
        {
            var product = QueryService.Load(guid);

            DeleteProduct.Execute(product);

            return RedirectToAction("Overview", new { productGroupId = product.ProductGroup.Id, page = 1 });
        }
    }
}
