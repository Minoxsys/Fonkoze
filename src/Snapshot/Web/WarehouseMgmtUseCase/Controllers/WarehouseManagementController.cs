using System.IO;
using System.Web;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using MvcContrib;

namespace Web.WarehouseMgmtUseCase.Controllers
{
    public class WarehouseManagementController : Controller
    {
        public ActionResult Overview()
        {
           return View(new OutpostOverviewModel());
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase csvfile)
        {
            // Verify that the user selected a file
            if (csvfile != null && csvfile.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(csvfile.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                //csvfile.SaveAs(path);
            }
            // redirect back to the index action to show the form once again
            return this.RedirectToAction(c => c.Overview());
        }
    }
}
