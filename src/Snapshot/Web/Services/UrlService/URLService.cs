using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Services.UrlService
{
    public class URLService : IURLService
    {
        public string GetEmailLinkUrl(UrlHelper helper, string data)
        {
            return GetApplicationPath() + helper.Action("Response", "EmailRequest") + "?id=" + data;
        }

        public string GetApplicationPath()
        {
            string host = (HttpContext.Current.Request.Url.IsDefaultPort) ? HttpContext.Current.Request.Url.Host : HttpContext.Current.Request.Url.Authority;
            return host = String.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, host);
        }
    }
}