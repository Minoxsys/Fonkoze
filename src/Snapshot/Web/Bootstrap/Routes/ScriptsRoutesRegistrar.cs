using System.Web.Routing;
using System.Web.Mvc;
namespace Web.Bootstrap.Routes
{
	public class ScriptsRoutesRegistrar
	{
		public static string JAVASCRIPT = "Javascript";
		public static string CSS = "Stylesheet";

		public static void Register( RouteCollection routes )
		{

			var version = AppSettings.ScriptsVersion;

			routes.MapRoute(JAVASCRIPT,
				"js/{group}/" + version,
				new
				{
					controller = "Javascript",
					action = "Index"
				});
			routes.MapRoute(CSS,
				"css/{group}-" + version,
				new
				{
					controller = "Css",
					action = "Index"
				});
		}
	}
}