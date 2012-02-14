using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.RoleManager
{
    public class RoleManagerListForJsonOutput
    {
        public RoleReferenceModel[] Roles { get; set; }
        public int TotalItems { get; set; }
    }
}