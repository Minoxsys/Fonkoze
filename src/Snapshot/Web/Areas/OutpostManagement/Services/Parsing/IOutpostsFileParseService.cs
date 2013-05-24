using System;
using System.IO;
using Web.Areas.OutpostManagement.Models.Outpost;
namespace Web.Areas.OutpostManagement.Services
{
    public interface IOutpostsFileParseService
    {
        OutpostsParseResult ParseStream(Stream stream);
    }
}
