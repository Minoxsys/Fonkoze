using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Services;
using Autofac;

namespace Web.Bootstrap.Container
{
    public class FileRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<FileService>().As<IImportService>();
        }
    }
}