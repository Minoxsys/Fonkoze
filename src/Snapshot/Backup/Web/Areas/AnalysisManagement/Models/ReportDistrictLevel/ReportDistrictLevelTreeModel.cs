using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.ReportDistrictLevel
{
    public class ReportDistrictLevelTreeModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String ProductLevelSum { get; set; }
        public Guid ParentId { get; set; }

        public bool leaf { get; set; }
        public bool expanded { get; set; }

        public List<ReportDistrictLevelTreeModel> children { get; set; }

        public ReportDistrictLevelTreeModel()
        {
            this.children = new List<ReportDistrictLevelTreeModel>();
        }
    }
}