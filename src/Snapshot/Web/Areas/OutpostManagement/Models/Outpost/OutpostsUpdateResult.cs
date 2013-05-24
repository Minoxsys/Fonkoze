using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Parsing.Outpost;

namespace Web.Areas.OutpostManagement.Services
{
    public class OutpostsUpdateResult
    {
        public bool Success { get; set; }
        public IList<IParsedOutpost> FailedOutposts { get; set; }
    }
}