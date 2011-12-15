using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class ClientModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }        

        public ClientModel()
        { }
    }
}