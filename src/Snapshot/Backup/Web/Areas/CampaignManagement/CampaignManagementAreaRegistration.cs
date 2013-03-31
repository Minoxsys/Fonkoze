using System.Web.Mvc;

namespace Web.Areas.CampaignManagement
{
    public class CampaignManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "CampaignManagement_default";
        public override string AreaName
        {
            get
            {
                return "CampaignManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "CampaignManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
