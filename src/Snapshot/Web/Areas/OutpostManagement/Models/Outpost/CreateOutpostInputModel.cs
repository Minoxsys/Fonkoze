using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
	public class CreateOutpostInputModel
	{
		public Guid? CountryId { get; set; }
		public Guid? RegionId { get; set; }
		public Guid? DistrictId { get; set; }
		public Guid? WarehouseId { get; set; }

		public string Name { get; set; }
		public string Coordinates { get; set; }

		public bool? IsWarehouse { get; set; }
	}
}