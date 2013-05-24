using System;
using System.Collections.Generic;
using Web.Models.Parsing.Outpost;
namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public interface IOutpostsParseResult
    {
        List<IParsedOutpost> ParsedOutposts { get; set; }
        bool Success { get; set; }
    }
}
