using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public class ToModalJsonActionResponse : JsonActionResponse
    {
        public bool CloseModal { get; set; }
    }
}