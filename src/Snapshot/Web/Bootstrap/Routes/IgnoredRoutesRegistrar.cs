using System.Web.Routing;
using System.Web.Mvc;

namespace Web.Bootstrap.Routes
{
    public class IgnoredRoutesRegistrar
    {
        public static void Register(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});
            routes.IgnoreRoute("Assets/{*path}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}