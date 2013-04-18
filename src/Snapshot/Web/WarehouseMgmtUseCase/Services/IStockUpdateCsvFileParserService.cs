using System.IO;
using Web.WarehouseMgmtUseCase.Model;

namespace Web.WarehouseMgmtUseCase.Services
{
    public interface IStockUpdateCsvFileParserService
    {
        CsvParseResult ParseStream(Stream dataStream);
    }
}
