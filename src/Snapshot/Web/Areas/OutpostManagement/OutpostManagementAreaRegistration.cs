using System.Web.Mvc;

namespace Web.Areas.OutpostManagement
{
    public class OutpostManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "OutpostManagement_default";
        public const string AREA_NAME = "OutpostManagement";
        
        //public static object DEFAULT_ROUTE { get; set; }

        //DEFAULT_ROUTE = "OutpostManagement_default";

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
                "OutpostManagement_default",
                "OutpostManagement/{controller}/{action}/{id}",
                new { action = "Overview", id = UrlParameter.Optional }
            );
        }

    }
}
