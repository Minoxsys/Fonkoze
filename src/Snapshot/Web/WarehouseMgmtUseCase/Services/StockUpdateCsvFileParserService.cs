using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using Web.Models.Parsing;
using Web.WarehouseMgmtUseCase.Model;

namespace Web.WarehouseMgmtUseCase.Services
{
    public class StockUpdateCsvFileParserService : IStockUpdateCsvFileParserService
    {
        private const string GenericProductGroupCode = "ALL";

        public CsvParseResult ParseStream(Stream dataStream)
        {
            try
            {
                var list = new List<IParsedProduct>();

                using (var reader = new CsvReader(new StreamReader(dataStream), false))
                {
                    reader.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;
                    if (reader.FieldCount == 3)
                    {
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
                    else
                    {
                        while (reader.ReadNextRecord())
                        {
                            list.Add(new ParsedProduct
                                {
                                    ProductGroupCode = GenericProductGroupCode,
                                    ProductCode = reader[0],
                                    StockLevel = int.Parse(reader[1])
                                });
                        }
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