using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
	public class GetWarehousesOutputModel
	{
		public class WarehouseModel
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}

		public GetWarehousesOutputModel()
		{
			Warehouses = new WarehouseModel[] { };
		}
		public WarehouseModel[] Warehouses { get; set; }
	}
}