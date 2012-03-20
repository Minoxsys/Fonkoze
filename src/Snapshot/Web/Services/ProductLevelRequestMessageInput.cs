using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Core.Domain;

namespace Web.Services
{
    public class ProductLevelRequestMessageInput
    {
        public Outpost Outpost { get; set; }
        public Contact Contact { get; set; }
        public ProductGroup ProductGroup { get; set; }
        public Client Client { get; set; }
        public User ByUser { get; set; }
        public List<Product> Products { get; set; }

        public ProductLevelRequest ProductLevelRequest { get; set; }
    }
}