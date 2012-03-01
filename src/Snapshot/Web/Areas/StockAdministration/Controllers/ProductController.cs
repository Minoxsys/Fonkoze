using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Web.Areas.StockAdministration.Models.Product;
using AutoMapper;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Client;
using Core.Domain;
using Web.Models.Shared;

namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductController : Controller
    {
        public IQueryService<Product> QueryService { get; set; }

        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }

        public IQueryService<OutpostHistoricalStockLevel> QueryOutpostStockLevelHystorical { get; set; }

        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public ISaveOrUpdateCommand<Product> SaveOrUpdateProduct { get; set; }

        public IDeleteCommand<Product> DeleteProduct { get; set; }

        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        private Client _client;
        private User _user;

        private const string TEMP_DATA_ERROR_HISTORY = "errorHistory";
        private const string TEMP_DATA_ERROR_CURRENT = "error";

        public ActionResult Overview()
        {
            return View();
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

        public JsonResult GetProductGroups()
        {
            LoadUserAndClient();

            var productGroups = QueryProductGroup.Query().Where(p => p.Client == _client).ToList();
            var productGroupModelList = new List<ProductGroupModel>();

            foreach (var productGroup in productGroups)
            {
                var productGroupModel = new ProductGroupModel();
                productGroupModel.Id = productGroup.Id;
                productGroupModel.Name = productGroup.Name;
                productGroupModelList.Add(productGroupModel);
            }

            return Json(new
            {
                productGroups = productGroupModelList,
                TotalItems = productGroupModelList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts(ProductIndexModel indexModel)
        {
            LoadUserAndClient();

            var products = QueryService.Query()
                                       .Where(p => p.Client == _client);

            if (indexModel.ProductGroupId != null)
            {
                products = products.Where(it => it.ProductGroup.Id == indexModel.ProductGroupId.Value);
            }

            if (!string.IsNullOrWhiteSpace(indexModel.SearchName))
            {
                products = products.Where(it => it.Name.Contains(indexModel.SearchName));
            }

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Product>>>()
            {
                { "Name-ASC", () => products.OrderBy(it => it.Name) },
                { "Name-DESC", () => products.OrderByDescending(c => c.Name) },
                { "ProductGroupName-ASC", () => products.OrderBy(it => it.ProductGroup.Name) },
                { "ProductGroupName-DESC", () => products.OrderByDescending(it => it.ProductGroup.Name) }
            };

            products = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            int totalItems = products.Count();
            products = products.Take(indexModel.Limit.Value)
                               .Skip(indexModel.Start.Value);

            var productList = new List<ProductModel>();

            foreach (var product in products.ToList())
            {
                var productModel = new ProductModel();
                CreateMappings();
                Mapper.Map(product, productModel);
                productModel.ProductGroupName = product.ProductGroup.Name;
                productModel.ProductGroupId = product.ProductGroup.Id.ToString();
                productList.Add(productModel);
            }

            return Json(new ProductIndexOutputModel
            {
                products = productList,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult Create(ProductInputModel inputProductModel)
        {
            LoadUserAndClient();
            if ((string.IsNullOrEmpty(inputProductModel.Name)) && (string.IsNullOrEmpty(inputProductModel.SMSReferenceCode)))
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        Message = "The product has not been saved!",
                        CloseModal = true
                    });
            }

            CreateMappings();

            var products = QueryService.Query()
                                       .Where(it => it.Client == _client)
                                       .Where(it => it.ProductGroup.Id == inputProductModel.ProductGroup.Id);

            if (products.Where(it=>it.Name == inputProductModel.Name).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        Message = string.Format("The Product Group already contains a product with the name {0}! Please insert a different name!", inputProductModel.Name),
                        CloseModal = false
                    });
            }

            if (products.Where(it => it.SMSReferenceCode == inputProductModel.SMSReferenceCode).Count() > 0)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       Message = string.Format("The Product Group already contains a product with SMS Reference code {0}! Please insert a different SMS Reference Code!", inputProductModel.SMSReferenceCode),
                       CloseModal = false
                   });
 
            }
            var product = new Product();
            Mapper.Map(inputProductModel, product);

            product.ProductGroup = QueryProductGroup.Load(inputProductModel.ProductGroup.Id);

            product.ByUser = _user;
            product.Client = _client;

            SaveOrUpdateProduct.Execute(product);

            return Json(
                new ToModalJsonActionResponse
                {
                    Status = "Success",
                    Message = "The product has been saved!",
                    CloseModal = true
                });
        }

        [HttpPost]
        public JsonResult Edit(ProductInputModel inputProductModel)
        {
            LoadUserAndClient();

            if ((string.IsNullOrEmpty(inputProductModel.Name)) && (string.IsNullOrEmpty(inputProductModel.SMSReferenceCode)))
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        Message = "The product has not been updated!",
                        CloseModal = true
                    });
            }
            var products = QueryService.Query()
                                       .Where(it => it.Client == _client && it.Id != inputProductModel.Id)
                                       .Where(it => it.ProductGroup.Id == inputProductModel.ProductGroup.Id);
            if (products.Where(it=>it.Name == inputProductModel.Name).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        Message = string.Format("The Product Group already contains a product with the name {0}! Please insert a different name!",inputProductModel.Name),
                        CloseModal = false
                    });
            }

            if (products.Where(it => it.SMSReferenceCode == inputProductModel.SMSReferenceCode).Count() > 0)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       Message = string.Format("The Product Group already contains a product with SMS Reference code {0}! Please insert a different SMS Reference Code!", inputProductModel.SMSReferenceCode),
                       CloseModal = false
                   });

            }

            CreateMappings();

            var product = QueryService.Load(inputProductModel.Id);
            Mapper.Map(inputProductModel, product);

            product.ProductGroup = QueryProductGroup.Load(inputProductModel.ProductGroup.Id);

            product.ByUser = _user;
            product.Client = _client;

            SaveOrUpdateProduct.Execute(product);

            return Json(
                new ToModalJsonActionResponse
                {
                    Status = "Success",
                    Message = "The product has been updated!",
                    CloseModal = true
                });
        }

        [HttpPost]
        public JsonResult Delete(Guid guid)
        {
            var product = QueryService.Load(guid);

            if (product != null)
            {
                var outpostStockLevel = QueryOutpostStockLevel.Query().Where(it => it.Product.Id == guid).ToList();
                var outpostStockLevelHystorical = QueryOutpostStockLevelHystorical.Query().Where(it => it.ProductId == guid).ToList();

                if (outpostStockLevel.Count > 0)
                {
                    return Json(
                        new JsonActionResponse
                        {
                            Status = "Error",
                            Message = string.Format("The product {0} has stock level available, so it can not be deleted", product.Name),
                        });
                }
                if (outpostStockLevelHystorical.Count > 0)
                {
                    return Json(
                        new JsonActionResponse
                        {
                            Status = "Error",
                            Message = string.Format("The product {0} has stock level history available , so it can not be deleted", product.Name),
                        });
                }

                DeleteProduct.Execute(product);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = string.Format("The product {0} has been deleted!", product.Name),
                });
        }
    }
}