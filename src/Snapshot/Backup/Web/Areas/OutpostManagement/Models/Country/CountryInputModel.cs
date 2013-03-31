using System;
using System.ComponentModel.DataAnnotations;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryInputModel
    {

        [Required(ErrorMessage = "Please enter the name of the country !")]
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string PhonePrefix { get; set; }
        public ClientModel Client { get; set; }
        public Guid Id { get; set; }
    }
}