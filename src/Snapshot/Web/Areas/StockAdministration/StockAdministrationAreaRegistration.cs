using System.Web.Mvc;

namespace Web.Areas.StockAdministration
{
    public class StockAdministrationAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "StockAdministration_default";
      
        public override string AreaName
        {
            get
            {
                return "StockAdministration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "StockAdministration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
