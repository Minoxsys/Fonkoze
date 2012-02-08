using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ClientManager
{
    public class ClientIndexOutputModel
    {
        public ClientManagerOutputModel[] Clients { get; set; }
        public int TotalItems { get; set; }
    }
}