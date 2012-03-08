using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Web.Services;

namespace Web.Bootstrap.Container
{
    public class SmsGatewayRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<SmsGatewayService>().As<ISmsGatewayService>();

            container.RegisterType<ProductLevelRequestMessagesDispatcherService>().As<IProductLevelRequestMessagesDispatcherService>();

            container.RegisterType<SmsRequestService>().As<IProductLevelRequestMessageSenderService>();

            container.RegisterType<EmailRequestService>().As<IProductLevelRequestMessageSenderService>();
        }
    }
}