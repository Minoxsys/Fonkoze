using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ClientManager
{
    public class ClientManagerOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int NoOfUsers { get; set; }
    }
}