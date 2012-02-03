using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public class JsonActionResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<String> RequiredFields { get; set; }

        public JsonActionResponse()
        {
            RequiredFields = new List<string>();
        }
    }
}