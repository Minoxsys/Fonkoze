using System;
using System.Collections.Generic;

namespace Web.Areas.StockAdministration.Models.OutpostStockLevel
{
    public class OutpostStockLevelCurrentTreeModel
    {
        public String Name { get; set; }
        public Guid Id { get; set; }
        public List<OutpostStockLevelCurrentTreeModel> children { get; set; }
        public int ProductLevel { get; set; }
        public String SMSCode { get; set; }
        public String LastUpdate { get; set; }
        public String UpdateMethod { get; set; }
        public int PreviousLevel { get; set; }
        public String Description { get; set; }
        public String ProductGroupName { get; set; }
        public String OutpostName { get; set; }

        public bool expanded { get; set; }
        public bool leaf { get; set; }

        public OutpostStockLevelCurrentTreeModel()
        {
            children = new List<OutpostStockLevelCurrentTreeModel>();
        }
    }
}