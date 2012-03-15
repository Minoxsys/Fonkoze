using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Services.Paths;

namespace Web.Services.Etag
{
    public interface IETagService
    {
        string Generate(string absolutePath);
    }
}