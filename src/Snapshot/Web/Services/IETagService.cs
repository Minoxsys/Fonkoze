using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services
{
    public interface IETagService
    {
        string Generate(string absolutePath);
    }
}