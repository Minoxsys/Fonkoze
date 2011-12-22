using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;

namespace Web.Areas.StockAdministration.Models.StockItem
{
    public class StockItemOutputModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public String SMSReferenceCode { get; set; }
        public StockGroupModel StockGroup { get; set; }
        public OutpostModel Outpost { get; set; }
        public List<SelectListItem> StockGroups { get; set; }
        public List<SelectListItem> OutpostList { get; set; }

        public IQueryService<StockGroup> QueryStockGroup { get; set; }
        public IQueryService<Outpost> QueryOutposts { get; set; }

        public StockItemOutputModel(IQueryService<StockGroup> queryStockGroup, IQueryService<Outpost> queryOutpost)
        {
            this.StockGroup = new StockGroupModel();
            this.StockGroups = new List<SelectListItem>();
            this.OutpostList = new List<SelectListItem>();

            this.QueryStockGroup = queryStockGroup;
            this.QueryOutposts = queryOutpost;

            var stockGroups = QueryStockGroup.Query();
            var outposts = QueryOutposts.Query();

            if (stockGroups.ToList().Count > 0)
            {
                foreach (StockGroup stockGroup in stockGroups)
                {
                    this.StockGroups.Add(new SelectListItem { Text = stockGroup.Name, Value = stockGroup.Id.ToString() });
                    
                }
            }

            if (outposts.ToList().Count > 0)
            {
                foreach (Outpost outpost in outposts)
                {
                    this.OutpostList.Add(new SelectListItem { Text = outpost.Name, Value = outpost.Id.ToString() });
                }
            }
        }

        public class OutpostModel
        {
            public Guid Id { get; set;}
            
        }
    }
}