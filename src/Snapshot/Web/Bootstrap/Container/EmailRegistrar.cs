using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Web.Services.EmailService;

namespace Web.Bootstrap.Container
{
    public class EmailRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<EmailService>().As<IEmailService>();
        }
    }
}