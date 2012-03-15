using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Web.Services.Paths
{
    public class PathService : IPathService
    {
        public bool Exists(string absolutePath)
        {
            return Directory.Exists(absolutePath);
        }

        public string[] GetDirectoryFiles(string absolutePath)
        {
            return Directory.GetFiles(absolutePath);
        }

        public DateTime GetLastWriteTime(string absolutePath)
        {
            return new DirectoryInfo(absolutePath).LastWriteTime;
        }

    }
}