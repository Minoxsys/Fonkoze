using System.Web.Mvc;

namespace Web.Areas.MessagesManagement
{
    public class MessagesManagementAreaRegistration : AreaRegistration
    {
        public const string DefaultRoute = "MessagesManagement_default";

        public override string AreaName
        {
            get
            {
                return "MessagesManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MessagesManagement_default",
                "MessagesManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
