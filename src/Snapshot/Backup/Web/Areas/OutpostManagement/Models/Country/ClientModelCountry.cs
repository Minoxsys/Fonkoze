using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class ClientModelCountry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }        

        public ClientModelCountry()
        { }
    }
}