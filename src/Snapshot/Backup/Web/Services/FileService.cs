using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Web.Services
{
    public class FileService : IImportService
    {
        public Stream GetStream(string filePath)
        {
            Stream fileStream = new FileStream(filePath, FileMode.Open); 

            return fileStream;
        }

        public bool IsValidPath(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public string GetEmployeeFile()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Imports\\Employees.csv");
        }


        public DataTable GetDataTable(string filePath)
        {
            string file = Path.GetFileName(filePath);
            string dir = Path.GetFileName(filePath);

            //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dir.Trim() + ";Extended Properties=\"Excel8.0;HDR=Yes;IMEX=1\";";
            string connString = "Driver={Microsoft Text Driver (*.txt; *.csv)}; Dbq=" + dir.Trim() + "; Extensions=asc,csv,tab,txt; HDR=Yes; Security Info=False";
            System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(connString.Trim());
            
            conn.Open();

            string query = "SELECT * FROM " + "[" + file + "]";

            DataTable dTable = new DataTable();
            System.Data.Odbc.OdbcDataAdapter dAdapter = new System.Data.Odbc.OdbcDataAdapter(query, conn);

            dAdapter.Fill(dTable);

            dAdapter.Dispose();
            conn.Close();

            return dTable;
        }
    }
}
