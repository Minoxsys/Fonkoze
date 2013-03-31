using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
	public class EditOutpostInputModel :CreateOutpostInputModel
	{
		public Guid? EntityId { get; set; }
	}
}