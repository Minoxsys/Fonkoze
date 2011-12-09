using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core;
using Core.Services;
using Core.Services.Implementations;
using Autofac;

namespace Web.Bootstrap.Container
{
    /// <summary>
    /// This class registers the implementations of minification services
    /// </summary>
    public static class MinificationRegistrar
    {
        private const string DEFAULT_CACHE_MANAGER = "DefaultCacheManager";
        private const string CSS_COMPRESSION_SERVICE_NAME = "CssCompressionService";
        private const string JS_COMPRESSION_SERVICE_NAME = "JsCompressionService";

        public static void RegisterWith( ContainerBuilder container )
        {
            container.RegisterType<MimeTypeResolverService>();
            container.RegisterType<WebApplicationFileService>();
            container.RegisterType<MicrosoftMinifierCssCompresionService>().As<ICssScriptCompressionService>();
            container.RegisterType<MicrosoftMinifierJavascriptCompressionService>().As<IJavaScriptCompressionService>();

            container.RegisterType<CssConfigurationProviderService>().As < ICssProviderService>();//(CSS_COMPRESSION_SERVICE_NAME);
            container.RegisterType<JavaScriptConfigurationProviderService>().As<IJavaScriptProviderService>();//(JS_COMPRESSION_SERVICE_NAME);

        }
    }
}