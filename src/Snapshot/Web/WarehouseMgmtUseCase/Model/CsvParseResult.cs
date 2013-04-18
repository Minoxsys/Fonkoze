using System.Collections.Generic;
using Web.Models.Parsing;

namespace Web.WarehouseMgmtUseCase.Model
{
    public class CsvParseResult : IParseResult
    {
        public bool Success { get; set; }
        public List<IParsedProduct> ParsedProducts { get; set; }
    }
}