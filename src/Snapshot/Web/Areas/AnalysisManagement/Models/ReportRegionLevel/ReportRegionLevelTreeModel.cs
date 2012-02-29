using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportRegionLevel
{
    public class ReportRegionLevelTreeModel
    {
        public String Name { get; set; }
        public String ProductLevelSum { get; set; }

        public bool leaf { get; set; }
        public bool expanded { get; set; }

        public List<ReportRegionLevelTreeModel> children { get; set; }

        public ReportRegionLevelTreeModel()
        {
            this.children = new List<ReportRegionLevelTreeModel>();
        }

    }
}