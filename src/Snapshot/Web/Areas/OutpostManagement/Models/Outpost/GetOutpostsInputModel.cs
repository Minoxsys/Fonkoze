using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
	public class GetOutpostsInputModel
	{

		public string _dc { get; set; }

		public int? page { get; set; }

		public int? start { get; set; }
		public int? limit { get; set; }

		public string sort { get; set; }
		public string dir { get; set; }

		public Guid? countryId { get; set; }
		public Guid? regionId { get; set; }
		public Guid? districtId { get; set; }

        public string search { get; set; }
    }
}