using System;
namespace Core.Services
{
    public interface IWebApplicationFileService
    {
        System.IO.StreamReader GetReaderFor(System.IO.FileInfo fileInfo);
        System.IO.FileInfo RetrieveFromPartialPath(string filePath);
    }
}
