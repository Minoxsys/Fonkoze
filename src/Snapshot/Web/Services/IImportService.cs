using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Web.Services
{
    public interface IImportService
    {
        Stream GetStream(string filePath);
        bool IsValidPath(string filePath);
        DataTable GetDataTable(string filePath);
        string GetEmployeeFile();
    }
}
