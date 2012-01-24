using System.Web.Routing;
using System.Web.Mvc;
namespace Web.Bootstrap.Routes
{
	public class ScriptsRoutesRegistrar
	{
		public static void Register( RouteCollection routes )
		{

			var version = AppSettings.ScriptsVersion;

			routes.MapRoute("Javascript",
				"js/{group}/_" + version,
				new
				{
					controller = "Javascript",
					action = "Index"
				});
			routes.MapRoute("Css",
				"css/{group}/" + version,
				new
				{
					controller = "Css",
					action = "Index"
				});
		}
	}
}