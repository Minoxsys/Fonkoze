using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Bootstrap;

namespace Web.Services
{
    public class URLService : IURLService
    {
        public string GetEmailLinkUrl(string data)
        {
           
            return AppSettings.EmailResponseUrl + "?id=" + data;
        }             
    }
}