using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Bootstrap.Container;
using Autofac;
using System.Reflection;
using Web.Controllers;
using Autofac.Features.ResolveAnything;

namespace Web.Bootstrap.Container
{
    public class ContainerRegistrar
    {

        public static void Register(ContainerBuilder container)
        {
            AutoWireControllerProperties(container);

            AuthRegistrar.Register(container);

            PersistenceRegistrar.Register(container);

            ServicesConventionsRegistrar.Register(container);

            container.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource(type =>
                    type.Assembly.FullName.StartsWith("Web")
                )
            );
        }

        private static void AutoWireControllerProperties(ContainerBuilder container)
        {
            var types = typeof(HomeController).Assembly.GetTypes();

            types.ToList().ForEach(it =>
            {
                if (it.BaseType == typeof(Controller))
                {
                    container.RegisterType(it).PropertiesAutowired();
                }

            }
                );

        }
    }
}