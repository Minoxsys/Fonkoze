using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Outpost;

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
        public ProductGroupModel ProductGroup { get; set; }
        public string UpdateMethod { get; set; }
        public int PreviousStockLevel { get; set; }
        public int StockLevel { get; set; }
        public OutpostModel Outpost { get; set; }
        public List<SelectListItem> ProductGroups { get; set; }

        public IQueryService<Domain.ProductGroup> QueryProductGroup { get; set; }


        public ProductOutputModel(IQueryService<Domain.ProductGroup> queryStockGroup, IQueryService<Outpost> queryOutpost)
        {
            this.ProductGroup = new ProductGroupModel();
            this.ProductGroups = new List<SelectListItem>();
            this.Outpost = new OutpostModel();
            this.QueryProductGroup = queryStockGroup;


            var stockGroups = QueryProductGroup.Query();


            if (stockGroups.ToList().Count > 0)
            {
                foreach (Domain.ProductGroup stockGroup in stockGroups)
                {
                    this.ProductGroups.Add(new SelectListItem { Text = stockGroup.Name, Value = stockGroup.Id.ToString() });

                }
            }


        }

    }
}