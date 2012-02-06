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
using Core.Domain;
using Web.Models.Shared;


namespace Web.Areas.StockAdministration.Controllers
{
    public class ProductGroupController : Controller
    {
        public IQueryService<ProductGroup> QueryService { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }
        public ISaveOrUpdateCommand<ProductGroup> SaveOrUpdateProductGroup { get; set; }
        public IDeleteCommand<ProductGroup> DeleteCommand { get; set; }
        


        public ActionResult Overview()
        {
            return View();
        }

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

        [HttpPost]
        public JsonResult Edit(ProductGroupInputModel model)
        {
            if (model.Id == Guid.Empty)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "You must supply a prouctGroupId in order to edit the region."
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
                    productResults = QueryProduct.Query().Where(it => it.ProductGroup.Id == productGroup.Id);

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
            var pageSize = indexModel.limit.Value;
            var productGroupDataQuery = this.QueryService.Query();
            var productQuery = this.QueryProduct.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<ProductGroup>>>()
            {
                { "Name-ASC", () => productGroupDataQuery.OrderBy(c => c.Name) },
                { "Name-DESC", () => productGroupDataQuery.OrderByDescending(c => c.Name) },
                { "Coordinates-ASC", () => productGroupDataQuery.OrderBy(c => c.Description) },
                { "Coordinates-DESC", () => productGroupDataQuery.OrderByDescending(c => c.Description) },
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

        private int GetProductsNumber(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                var productQuery = this.QueryProduct.Query();
                if (productQuery != null)
                    return productQuery.Where(it => it.ProductGroup.Id == guid).Count();
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







        //public ViewResult EditProducts(Guid guid)
        //{
        //    var ProductGroupOutputModel = new ProductGroupOutputModel();

        //    var ProductGroup = QueryService.Load(guid);

        //    CreateMappings();

        //    Mapper.Map(ProductGroup, ProductGroupOutputModel);

        //    return View(ProductGroupOutputModel);
        //}

        //[HttpPost]
        //public ActionResult EditProducts(ProductGroupInputModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Edit", ProductGroupOutputModel);
        //    }

        //    CreateMappings();

        //    var ProductGroup = new ProductGroup();

        //    Mapper.Map(model, ProductGroup);

        //    SaveOrUpdateProductGroup.Execute(ProductGroup);

        //    return RedirectToAction("Overview");
        //}

        

        //public ViewResult CreateProductGroupForProduct(bool CreateCommingFromProductCreate, bool CreateCommingFromProductEdit, Guid? productId)
        //{
        //    var ProductGroupOutputModel = new ProductGroupOutputModel();
        //    TempData["FromProduct"] = CreateCommingFromProductCreate;
        //    TempData["FromProductEdit"] = CreateCommingFromProductEdit;
        //    if (productId != null)
        //    {
        //        TempData["ProductId"] = productId.Value.ToString();
        //    }
        //    return View("Create", ProductGroupOutputModel);
        //}
    }
}
