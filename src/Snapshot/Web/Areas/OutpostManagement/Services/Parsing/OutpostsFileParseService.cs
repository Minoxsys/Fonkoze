using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using LumenWorks.Framework.IO.Csv;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Models.Parsing.Outpost;

namespace Web.Areas.OutpostManagement.Services
{
    public class OutpostsFileParseService : IOutpostsFileParseService
    {
        public OutpostsParseResult ParseStream(Stream stream)
        {
            try
            {
                var list = new List<IParsedOutpost>();

                using (var reader = new CsvReader(new StreamReader(stream), false))
                {
                    reader.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;
                    while (reader.ReadNextRecord())
                    {
                        list.Add(new ParsedOutpost
                            {
                                Country = reader[0],
                                Region = reader[1],
                                District = reader[2],
                                Name = reader[3],
                                Longitude = reader[4],
                                Latitude = reader[5],
                                ContactDetail = reader[6]
                            });
                    }
                }

                return new OutpostsParseResult { Success = true, ParsedOutposts = list };
            }
            catch
            {
                return new OutpostsParseResult { Success = false };
            }
        }
    }
}