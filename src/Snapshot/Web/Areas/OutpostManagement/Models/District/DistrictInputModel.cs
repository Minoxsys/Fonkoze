using System;
using System.ComponentModel.DataAnnotations;
using Web.LocalizationResources;

namespace Web.Areas.OutpostManagement.Models.District
{
    public class DistrictInputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof (Strings), ErrorMessageResourceName = "DistrictInputModel_Name_Name_for_district_is_required")]
        public string Name { get; set; }

        public RegionInputModel Region { get; set; }
        public ClientInputModel Client { get; set; }
        public Guid ManagerId { get; set; }

        public DistrictInputModel()
        {
            Region = new RegionInputModel();
            Client = new ClientInputModel();
        }
        public class RegionInputModel
        {
            [Required(ErrorMessageResourceType = typeof (Strings), ErrorMessageResourceName = "RegionInputModel_Id_Region_is_required")]
            public Guid Id { get; set; }

            public Guid CountryId { get; set; }
        }

        public class ClientInputModel
        {
            public Guid Id { get; set; }
        }
    }
}