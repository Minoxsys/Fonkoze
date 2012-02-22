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

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }
        public JsonResult GetProductGroups()
        {
            var productGroups = QueryProductGroup.Query().ToList();           
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
                productGroups = productGroupModelList
            ,
                TotalItems = productGroupModelList.Count
            }, JsonRequestBehavior.AllowGet);
 
        }
        public JsonResult GetProducts(ProductIndexModel indexModel)
        {
            var products = QueryService.Query().Take(0);

            int pageSize = 0;

            if (indexModel.Limit != null)
                pageSize = indexModel.Limit.Value;

            if (indexModel.ProductGroupId != null)
            {
                if (indexModel.SearchName != null)
                {
                    products = QueryService.Query().Where(it => it.ProductGroup.Id == indexModel.ProductGroupId.Value && it.Name.Contains(indexModel.SearchName));
                }
                else
                {
                    products = QueryService.Query().Where(it => it.ProductGroup.Id == indexModel.ProductGroupId.Value);

                }

            }
            else
            {
                if (indexModel.SearchName != null)
                {
                    products = QueryService.Query().Where(it => it.Name.Contains(indexModel.SearchName));
                }
                else
                {
                    products = QueryService.Query();

                }
 
            }

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Product>>>()
            {
                { "Name-ASC", () => products.OrderBy(it=>it.Name) },
                { "Name-DESC", () => products.OrderByDescending(c => c.Name) },
                { "ProductGroupName-ASC", () => products.OrderBy(it=>it.ProductGroup.Name)},
                { "ProductGroupName-DESC", () => products.OrderByDescending(it=>it.ProductGroup.Name)}
            };

            products = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            int totalItems = products.Count();
            products = products.Take(pageSize)
                               .Skip(indexModel.Start.Value);
            var productList = new List<ProductModel>();

            foreach (var product in products.ToList())
            {
                var productModel = new ProductModel();
                CreateMappings();
                Mapper.Map(product, productModel);
                productModel.ProductGroupName = product.ProductGroup.Name;
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
          if ((string.IsNullOrEmpty(inputProductModel.Name)) && (string.IsNullOrEmpty(inputProductModel.SMSReferenceCode)))
                {
                    return Json(
                        new JsonActionResponse
                        {
                            Status = "Error",
                            Message = "The product has not been saved!",
                            CloseModal = true
                        });

                }
            
            CreateMappings();

            var products = QueryService.Query().Where(it => it.ProductGroup.Id == inputProductModel.ProductGroup.Id && it.Name == inputProductModel.Name).ToList();

            if (products.Count > 0)
            {
                return Json(
                        new JsonActionResponse
                        {
                            Status = "Error",
                            Message = string.Format("The Product Group {0} already contains a product with the name {1}! Please insert a different name!", products[0].ProductGroup.Name, products[0].Name),
                            CloseModal = false
                        });               

            }
            else
            {
                var product = new Product();
                Mapper.Map(inputProductModel, product);

                product.ProductGroup = QueryProductGroup.Load(inputProductModel.ProductGroup.Id);

                SaveOrUpdateProduct.Execute(product);

                return Json(
                        new JsonActionResponse
                        {
                            Status = "Success",
                            Message = "The product has been saved!",
                            CloseModal = true
                        });
            }
        }
              
        [HttpPost]
        public JsonResult Edit(ProductInputModel inputProductModel)
        {
            if ((string.IsNullOrEmpty(inputProductModel.Name)) && (string.IsNullOrEmpty(inputProductModel.SMSReferenceCode)))
            {
                return Json(
                    new JsonActionResponse
                    {
                        Status = "Error",
                        Message = "The product has not been updated!",
                        CloseModal = true
                    });

            }
            var products = QueryService.Query().Where(it => it.ProductGroup.Id == inputProductModel.ProductGroup.Id && it.Name == inputProductModel.Name && it.Id != inputProductModel.Id).ToList();

            if (products.Count > 0)
            {               
                return Json(
                        new JsonActionResponse
                        {
                            Status = "Error",
                            Message = string.Format("The Product Group {0} already contains a product with the name {1}! Please insert a different name!", products[0].ProductGroup.Name, products[0].Name),
                            CloseModal = false
                        });  

            }
            else
            {
                CreateMappings();

                var product = QueryService.Load(inputProductModel.Id);
                Mapper.Map(inputProductModel, product);
                product.ProductGroup = QueryProductGroup.Load(inputProductModel.ProductGroup.Id);
                SaveOrUpdateProduct.Execute(product);

                return Json(
                         new JsonActionResponse
                         {
                             Status = "Success",
                             Message = "The product has been updated!",
                             CloseModal = true
                         }); 
            }
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
