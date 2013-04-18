using System;
using LumenWorks.Framework.IO.Csv;
using System.Collections.Generic;
using System.IO;
using Web.Models.Parsing;
using Web.WarehouseMgmtUseCase.Model;

namespace Web.WarehouseMgmtUseCase.Services
{
    public class StockUpdateCsvFileParserService : IStockUpdateCsvFileParserService
    {
        public CsvParseResult ParseStream(Stream dataStream)
        {
            try
            {
                var list = new List<IParsedProduct>();

                using (var reader = new CsvReader(new StreamReader(dataStream), false))
                {
                    reader.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;
                    while (reader.ReadNextRecord())
                    {
                        list.Add(new ParsedProduct
                            {
                                ProductGroupCode = reader[0],
                                ProductCode = reader[1],
                                StockLevel = int.Parse(reader[2])
                            });
                    }
                }

                return new CsvParseResult {Success = true, ParsedProducts = list};
            }
            catch (Exception)
            {
                return new CsvParseResult {Success = false};
            }
        }
    }
}