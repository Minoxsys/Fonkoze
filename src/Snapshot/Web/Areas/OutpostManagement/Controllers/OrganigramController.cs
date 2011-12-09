using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Domain;

namespace Web.Areas.OnBoarding.Controllers
{
    public class OrganigramController : Controller
    {
		[Dependency]
		public Core.Persistence.IQueryService<Employee> EmployeeQueryService { get; set; }

        public JsonResult TeamLeaderQuickSearch(string term)
        {			
			var employees = EmployeeQueryService.Query();
			var query = from x in employees
						where x.Name.Contains(term)
						select new
						{
							label = x.Name,
							id = x.Id,
							Name = x.Name
						};
						
			return Json(query, JsonRequestBehavior.AllowGet);			
        }

    }
}
