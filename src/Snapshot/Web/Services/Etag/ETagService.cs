using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Services.Paths;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Web.Services.Etag
{
    public class ETagService : IETagService
    {
        public IPathService Path { get; set; }

        public string Generate(string absolutePath)
        {
            var stringBuilder = new StringBuilder();

            string[] files = Path.GetDirectoryFiles(absolutePath);

            files.ToList().ForEach(file =>
            {
                using (var lFileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (var lBinaryReader = new BinaryReader(lFileStream))
                    {
                        var cryptService = new MD5CryptoServiceProvider();
                        byte[] hash = cryptService.ComputeHash(lBinaryReader.BaseStream);
                        foreach (byte hex in hash)
                        {
                            stringBuilder.Append(hex.ToString("x2"));
                        }

                    }
                }
            });
            return stringBuilder.ToString();
        }
    }
}