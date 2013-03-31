using System.Web.Mvc;

namespace Web.Areas.AnalysisManagement
{
    public class AnalysisManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "AnalysisManagement_default";
        public override string AreaName
        {
            get
            {
                return "AnalysisManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "AnalysisManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
