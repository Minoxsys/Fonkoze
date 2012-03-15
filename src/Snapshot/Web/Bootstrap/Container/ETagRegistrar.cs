using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Web.Services.Etag;

namespace Web.Bootstrap.Container
{
    public class ETagRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<ETagService>().As<IETagService>();
        }
    }
}