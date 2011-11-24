﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using System.Web.Routing;
using System.Web.Mvc;

namespace Web.Bootstrap.Routes
{
	public class AssetRoutesRegistrar
	{
        public const string SHARED = "Shared";
		public static void Register( RouteCollection routes)
		{
		    
		    routes.MapRoute(SHARED,
				"shared/{*file}",
				new
				{
					controller = "Assets",
					action = "shared",
					file = UrlParameter.Optional
				}
				);
		}
	}
}