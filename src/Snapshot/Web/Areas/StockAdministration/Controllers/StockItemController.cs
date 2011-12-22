using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Persistence.Queries.StockItems;
using Web.Areas.StockAdministration.Models.StockItem;
using AutoMapper;

namespace Web.Areas.StockAdministration.Controllers
{
    public class StockItemController : Controller
    {
        public IQueryStockItem QueryStockItem { get; set; }

        public IQueryService<StockItem> QueryService { get; set; }

        public StockItemOutputModel StockItemOutputModel { get; set; }

        public IQueryService<StockGroup> QueryStockGroup { get; set; }

        public ISaveOrUpdateCommand<StockItem> SaveOrUpdateStockItem { get; set; }

        public IDeleteCommand<StockItem> DeleteStockItem { get; set; }

        public IQueryService<Outpost> QueryOutposts { get; set; }

        public ActionResult Overview()
        {
            var overviewModel = new StockItemOverviewModel();

            var stockItems = QueryStockItem.GetAll();

            if (stockItems.ToList().Count > 0)
            {
                foreach (StockItem stockItem in stockItems)
                {
                    CreateMappings();
                    var stockItemModel = new StockItemModel();
                    Mapper.Map(stockItem, stockItemModel);
                    overviewModel.StockItems.Add(stockItemModel);
                    
                }
            }

            return View(overviewModel);
        }

        private void CreateMappings()
        {
            Mapper.CreateMap<StockItemModel, StockItem>();
            Mapper.CreateMap<StockItem, StockItemModel>();

            Mapper.CreateMap<StockGroupModel, StockGroup>();
            Mapper.CreateMap<StockGroup, StockGroupModel>();

            Mapper.CreateMap<StockItemInputModel.StockGroupInputModel, StockGroup>();
            Mapper.CreateMap<StockGroup, StockItemInputModel.StockGroupInputModel>();

            Mapper.CreateMap<StockItemInputModel, StockItem>();
            Mapper.CreateMap<StockItem, StockItemInputModel>();

            Mapper.CreateMap<StockItem, StockItemOutputModel>();
            Mapper.CreateMap<StockItemOutputModel, StockItem>();

            Mapper.CreateMap<StockItemOutputModel.OutpostModel, Outpost>();
            Mapper.CreateMap<Outpost, StockItemOutputModel.OutpostModel>();
        }

        public ViewResult Create()
        {
            var stockItemOutputModel = new StockItemOutputModel(QueryStockGroup, QueryOutposts);
            return View(stockItemOutputModel);
        }

        [HttpPost]
        public ActionResult Create(StockItemInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var stockItemOutputModel = BuildStockItemOutputModelFromInputModel(model);

                return View("Create", stockItemOutputModel);
            }

            CreateMappings();

            var stockItem = new StockItem();
            Mapper.Map(model, stockItem);

            
            stockItem.StockGroup = QueryStockGroup.Load(model.StockGroup.Id);
            
            var outpost = QueryOutposts.Load(model.Outpost.Id);
            if (outpost != null)
                stockItem.Outposts.Add(outpost);

            SaveOrUpdateStockItem.Execute(stockItem);

            return RedirectToAction("Overview");
        }

        private StockItemOutputModel BuildStockItemOutputModelFromInputModel(StockItemInputModel model)
        {
            var stockItemOutputModel = new StockItemOutputModel(QueryStockGroup,QueryOutposts);
            stockItemOutputModel.Description = model.Description;
            stockItemOutputModel.Id = model.Id;
            stockItemOutputModel.LowerLimit = model.LowerLimit;
            stockItemOutputModel.UpperLimit = model.UpperLimit;
            stockItemOutputModel.Name = model.Name;
            stockItemOutputModel.SMSReferenceCode = model.SMSReferenceCode;
            stockItemOutputModel.StockGroup.Id = model.StockGroup.Id;
            if (model.StockGroup != null)
            {
                if (model.StockGroup.Id != null)
                {
                    if (stockItemOutputModel.StockGroups.Where(it => it.Value == model.StockGroup.Id.ToString()).ToList().Count > 0)
                        stockItemOutputModel.StockGroups.First(it => it.Value == model.StockGroup.Id.ToString()).Selected = true;
                }
            }
            if (model.Outpost != null)
            {
                if (model.Outpost.Id != null)
                {
                    if (stockItemOutputModel.OutpostList.Where(it => it.Value == model.Outpost.Id.ToString()).ToList().Count > 0)
                        stockItemOutputModel.OutpostList.First(it => it.Value == model.Outpost.Id.ToString()).Selected = true;
                }
            }
            return stockItemOutputModel;
        }

        public ViewResult Edit(Guid guid)
        {
            var stockItemOutputModel = new StockItemOutputModel(QueryStockGroup,QueryOutposts);

            var stockItem = QueryService.Load(guid);

            CreateMappings();

            Mapper.Map(stockItem, stockItemOutputModel);

            if (stockItemOutputModel.StockGroups.Where(it => it.Value == stockItem.StockGroup.Id.ToString()).ToList().Count > 0)
            {
                stockItemOutputModel.StockGroups.First(it => it.Value == stockItem.StockGroup.Id.ToString()).Selected = true; 
            }

            return View(stockItemOutputModel);
        }

        [HttpPost]
        public ActionResult Edit(StockItemInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var stockItemOutputModel = BuildStockItemOutputModelFromInputModel(model);
                return View("Edit", stockItemOutputModel);
            }

            CreateMappings();

            var stockItem = new StockItem();

            Mapper.Map(model, stockItem);

            stockItem.StockGroup = QueryStockGroup.Load(model.StockGroup.Id);

            if (model.Outpost != null)
            {
                var outpost = QueryOutposts.Load(model.Outpost.Id);
                if (outpost != null)
                    stockItem.Outposts.Add(outpost);
            }

            SaveOrUpdateStockItem.Execute(stockItem);

            return RedirectToAction("Overview");
        }

        [HttpPost]
        public ActionResult Delete(Guid guid)
        {
            var stockItem = QueryService.Load(guid);

            DeleteStockItem.Execute(stockItem);

            return RedirectToAction("Overview");
        }
    }
}
