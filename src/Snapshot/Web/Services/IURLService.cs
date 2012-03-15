using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Services
{
    public interface IURLService
    {
        string GetEmailLinkUrl(string data);
    }
}