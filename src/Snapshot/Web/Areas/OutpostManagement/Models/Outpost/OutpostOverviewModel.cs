
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;
using Domain;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostOverviewModel
    {
        
        public List<Domain.Country> Countries {get; set;}
        public List<Domain.Region> Regions {get; set;}
        public List<Domain.District> Districts {get; set;}
        public Guid Id;
        public List<OutpostModel> Outposts { get; set; }

        public OutpostOverviewModel()
        {
            Outposts = new List<OutpostModel>();
            
        }

        public string Error { get; set; }
   }
}