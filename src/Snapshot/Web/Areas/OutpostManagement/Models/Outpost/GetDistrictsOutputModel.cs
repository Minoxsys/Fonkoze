using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
	public class GetDistrictsOutputModel
	{
		public GetDistrictsOutputModel()
		{
			Districts = new DistrictModel[]{};
		}
		public DistrictModel[] Districts { get; set; }

		public class DistrictModel
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
		}
	}
}