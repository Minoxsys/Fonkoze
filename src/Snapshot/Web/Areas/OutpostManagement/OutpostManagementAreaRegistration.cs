using System.Web.Mvc;

namespace Web.Areas.OutpostManagement
{
    public class OutpostManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "OutpostManagement_default";
        public override string AreaName
        {
            get
            {
                return "OutpostManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "OutpostManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
