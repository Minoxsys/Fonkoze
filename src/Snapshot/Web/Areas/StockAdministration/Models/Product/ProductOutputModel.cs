using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Areas.StockAdministration.Models.ProductGroup;

namespace Web.Areas.StockAdministration.Models.Product
{
    public class ProductOutputModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public String SMSReferenceCode { get; set; }
        public ProductGroupModel StockGroup { get; set; }
        public OutpostModel Outpost { get; set; }
        public List<SelectListItem> StockGroups { get; set; }
        public List<SelectListItem> OutpostList { get; set; }

        public IQueryService<Domain.ProductGroup> QueryStockGroup { get; set; }
        public IQueryService<Outpost> QueryOutposts { get; set; }

        public ProductOutputModel(IQueryService<Domain.ProductGroup> queryStockGroup, IQueryService<Outpost> queryOutpost)
        {
            this.StockGroup = new ProductGroupModel();
            this.StockGroups = new List<SelectListItem>();
            this.OutpostList = new List<SelectListItem>();

            this.QueryStockGroup = queryStockGroup;
            this.QueryOutposts = queryOutpost;

            var stockGroups = QueryStockGroup.Query();
            var outposts = QueryOutposts.Query();

            if (stockGroups.ToList().Count > 0)
            {
                foreach (var stockGroup in stockGroups)
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