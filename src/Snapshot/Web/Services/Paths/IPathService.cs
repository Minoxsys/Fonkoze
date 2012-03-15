using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Web.Services.Paths
{
    public interface IPathService
    {
        bool Exists(string absolutePath);
        string[] GetDirectoryFiles(string absolutePath);
        DateTime GetLastWriteTime(string absolutePath);
    }
}