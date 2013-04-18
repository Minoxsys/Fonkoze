using System.Collections.Generic;

namespace Web.Models.Parsing
{
    public interface IParseResult
    {
        bool Success { get; set; }
        List<IParsedProduct> ParsedProducts { get; set; }
    }
}