using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Bootstrap.Routes
{
    public class UserManagerRegistrar
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapRoute("UserManager",
                "users/{action}/{id}",
                new
                {
                    controller = "UserManager",
                    action = "List",
                    id = UrlParameter.Optional
                });
        }
    }
}