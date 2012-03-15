using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Web.Services
{
    public interface IPathService
    {
        bool Exists(string absolutePath);
        DateTime GetLastWriteTime(string absolutePath);
    }
}