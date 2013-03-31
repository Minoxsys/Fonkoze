using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
	public class GetOutpostsOutputModel
	{
		public class OutpostModel
		{
			public string Id { get; set; }
			public string Name{get;set;}

			public string ContactMethod { get; set; }

			public string Coordinates { get; set; }

			public bool IsWarehouse { get; set; }

			public string WarehouseName { get; set; }
			public string WarehouseId { get; set; }
			public string CountryId { get; set; }
			public string RegionId { get; set; }
			public string DistrictId { get; set; }
		}

		public OutpostModel[] Outposts { get; set; }

		public int TotalItems { get; set; }
	}
}