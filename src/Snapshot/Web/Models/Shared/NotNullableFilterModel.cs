using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public class NotNullableFilterModel
    {
        public Guid countryId { get; set; }
        public Guid regionId { get; set; }
        public Guid districtId { get; set; }
        public Guid outpostId { get; set; }
    }
}