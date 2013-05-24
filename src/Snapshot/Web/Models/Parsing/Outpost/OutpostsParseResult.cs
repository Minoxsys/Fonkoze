using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Parsing.Outpost;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostsParseResult : IOutpostsParseResult
    {
        public bool Success { get; set; }
        public List<IParsedOutpost> ParsedOutposts { get; set; }
    }
}