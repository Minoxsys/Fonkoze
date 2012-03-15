using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Web.Services.Paths;

namespace Web.Bootstrap.Container
{
    public class DirectoryRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<PathService>().As<IPathService>();
        }
    }
}