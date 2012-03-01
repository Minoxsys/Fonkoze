using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Web.Areas.StockAdministration.Models.ProductGroup;
using AutoMapper;
using Web.Models.Shared;
using Core.Domain;


namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductGroupController : Controller
    {
        private Core.Domain.User _user;
        private Client _client;
        public IQueryService<ProductGroup> QueryService { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }
        public ISaveOrUpdateCommand<ProductGroup> SaveOrUpdateProductGroup { get; set; }
        public IDeleteCommand<ProductGroup> DeleteCommand { get; set; }
        

        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        public class ProductGroupOutputModel : JsonActionResponse
        {
            public string ProductGroupId { get; set; }

        }
        [HttpPost]
        public JsonResult Create(ProductGroupInputModel model)
        {
            LoadUserAndClient();
            if (string.IsNullOrEmpty(model.Name) && string.IsNullOrEmpty(model.Description))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The Product Group has not been saved!"
                   });
            }

            var queryProductGroupValidation = QueryService.Query().Where(p => p.Client == _client);
            if (queryProductGroupValidation.Where(it => it.Name == model.Name).Count() > 0)
            {
                return Json(
                    new ProductGroupOutputModel
                    {
                        Status = "Error",
                        Message = string.Format("There is already a product group with this name: {0} ! Please insert a different name!", model.Name)
                    });
            }

            CreateMappings();

            var productGroup = new ProductGroup();
            Mapper.Map(model, productGroup);

            productGroup.ByUser = _user;
            productGroup.Client = _client;

            SaveOrUpdateProductGroup.Execute(productGroup);

            return Json(
               new ProductGroupOutputModel
               {
                   Status = "Success",
                   ProductGroupId = productGroup.Id.ToString(),
                   Message = String.Format("Product Group {0} has been saved.", productGroup.Name)
               });
        }

        [HttpPost]
        public JsonResult Edit(ProductGroupInputModel model)
        {
            LoadUserAndClient();
            if (model.Id == Guid.Empty)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "You must supply a prouctGroupId in order to edit the region."
                   });
            }

            var queryProductGroupValidation = QueryService.Query().Where(p => p.Client == _client);
            if (queryProductGroupValidation.Where(it => it.Name == model.Name && it.Id != model.Id).Count() > 0)
            {
                return Json(
                    new ProductGroupOutputModel
                    {
                        Status = "Error",
                        Message = string.Format("There is already a product group with this name: {0} ! Please insert a different name!", model.Name)
                    });

            }

            CreateMappings();

            var productGroup = QueryService.Load(model.Id);

            Mapper.Map(model, productGroup);
            productGroup.ByUser = _user;

            SaveOrUpdateProductGroup.Execute(productGroup);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Product Group {0} has been saved.", productGroup.Name)
               });
        }

        [HttpPost]
        public JsonResult Delete(Guid? productGroupId)
        {
            if (productGroupId.HasValue == false)
            {
                return Json(new JsonActionResponse
                {
                    Status = "Error",
                    Message = "You must supply a pproductGroupId in order to remove the product group."
                });
            }

            var productGroup = QueryService.Load(productGroupId.Value);
            var productResults = QueryProduct.Query();

            if (productResults != null)
            {
                if (productGroup != null)
                {
                    productResults = productResults.Where(it => it.ProductGroup.Id == productGroup.Id);

                    if (productResults.ToList().Count != 0)
                    {
                        return Json(new JsonActionResponse
                        {
                            Status = "Error",
                            Message = String.Format("The Product Group {0} has {1} product(s) associated, so it can not be deleted.", productGroup.Name, productResults.ToList().Count)
                        });
                    }
                }

                DeleteCommand.Execute(productGroup);
            }
       
            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Product Group {0} was removed.", productGroup.Name)
                }); 
        }

        [HttpGet]
        public JsonResult GetProductGroups(ProductGroupIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var productGroupDataQuery = this.QueryService.Query();
            var productQuery = this.QueryProduct.Query()
                .Where(p=>p.Client == _client);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<ProductGroup>>>()
            {
                { "Name-ASC", () => productGroupDataQuery.OrderBy(c => c.Name) },
                { "Name-DESC", () => productGroupDataQuery.OrderByDescending(c => c.Name) },
            };

            productGroupDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                productGroupDataQuery = productGroupDataQuery.Where(it => it.Name.Contains(indexModel.searchValue));
            }

            var totalItems = productGroupDataQuery.Count();

            productGroupDataQuery = productGroupDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var productGroupModelListProjection = (from productGroup in productGroupDataQuery.ToList()
                                                   select new ProductGroupModel
                                                   {
                                                       Id = productGroup.Id,
                                                       Name = productGroup.Name,
                                                       Description = productGroup.Description,
                                                       ReferenceCode = productGroup.ReferenceCode,
                                                       ProductsNo = GetProductsNumber(productGroup.Id)
                                                   }).ToArray();

    
            return Json(new ProductGroupIndexOutputModel
            {
                ProductGroups = productGroupModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private int GetProductsNumber(Guid? guid)
        {
            if (guid.HasValue)
            {
                var productQuery = this.QueryProduct.Query();
                if (productQuery != null)
                    return productQuery.Where(it => it.ProductGroup.Id == guid.Value).Count();
            }
            return 0;
        }

        private void CreateMappings()
        {
            Mapper.CreateMap<ProductGroupModel, ProductGroup>();
            Mapper.CreateMap<ProductGroup, ProductGroupModel>();

            Mapper.CreateMap<ProductGroupInputModel, ProductGroup>();
            Mapper.CreateMap<ProductGroupOutputModel, ProductGroupInputModel>();

            Mapper.CreateMap<ProductGroup, ProductGroupOutputModel>();
            Mapper.CreateMap<ProductGroupOutputModel, ProductGroup>();
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
