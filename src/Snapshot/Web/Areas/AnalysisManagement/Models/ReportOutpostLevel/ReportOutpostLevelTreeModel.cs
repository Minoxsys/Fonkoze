using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportOutpostLevel
{
    public class ReportOutpostLevelTreeModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String ProductLevelSum { get; set; }
        public int LowerLimit { get; set; }
        public Guid ParentId { get; set; }

        public bool leaf { get; set; }
        public bool expanded { get; set; }

        public List<ReportOutpostLevelTreeModel> children { get; set; }

        public ReportOutpostLevelTreeModel()
        {
            this.children = new List<ReportOutpostLevelTreeModel>();
        }
    }
}