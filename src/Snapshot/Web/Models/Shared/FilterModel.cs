using System;

namespace Web.Models.Shared
{
    public class FilterModel
    {
        public Guid? countryId { get; set; }
        public Guid? regionId { get; set; }
        public Guid? districtId { get; set; }
        public Guid? outpostId { get; set; }
    }
}