﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Web.Bootstrap.Routes;
using MvcContrib.TestHelper;
using System.Web.Mvc;
using NUnit.Framework;
using Web.Controllers;


namespace UnitTests.Routes
{
	[TestFixture]
	public class ScriptsRegistrar_Tests
	{
		[SetUp]
		public void BeforeEach()
		{
			var routes = RouteTable.Routes;

			routes.Clear();

			ScriptsRoutesRegistrar.Register(routes);

		}
		[Test]
		public void MapsRoute_Css()
		{
			"~/css/group/ver1".ShouldMapTo<CssController>(action => action.Index("group"));
		}

		[Test]
		public void MapRoute_Js()
		{
			"~/js/group/_ver1".ShouldMapTo<JavascriptController>(action => action.Index("group"));
		}
	}
}
