using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using Web.Services;

namespace Web.Bootstrap.Container
{
    public class FileRegistrar
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IImportService, FileService>();
        }
    }
}