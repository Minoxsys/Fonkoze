using System;


namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryModelInput
    {

        public Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }
        public virtual string PhonePrefix { get; set; }
    }
}