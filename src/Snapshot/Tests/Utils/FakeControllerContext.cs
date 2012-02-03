using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcContrib.TestHelper;

namespace Tests
{
    internal static class FakeControllerContext
    {
        static readonly TestControllerBuilder _builder = new TestControllerBuilder(new Utils.MoqFactory());

        public static TestControllerBuilder Builder { get { return _builder; } }


        public static void Initialize(Controller controller)
        {
            controller.ValueProvider = new FormCollection{}.ToValueProvider();
            _builder.InitializeController(controller);
        }
    }
}
