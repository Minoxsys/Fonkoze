using System.Web.Routing;

namespace Web.Bootstrap.Routes
{
    public static class RoutesRegistrar
    {
        public static void Register()
        {
            var routes = RouteTable.Routes;

            IgnoredRoutesRegistrar.Register(routes);

            // immediately after this comment add any routes registrar call
            ScriptsRoutesRegistrar.Register(routes);
            AssetRoutesRegistrar.Register(routes);

            RoleManagerRegistrar.Register(routes);
            UserManagerRegistrar.Register(routes);

            // this is allways the last to be registered
            DefaultRouteRegistrar.Register(routes);
        }
    }
}