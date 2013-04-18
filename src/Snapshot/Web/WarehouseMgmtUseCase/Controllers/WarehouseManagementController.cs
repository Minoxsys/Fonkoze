using System;
using MvcContrib;
using System.Web;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.LocalizationResources;
using Web.WarehouseMgmtUseCase.Services;

namespace Web.WarehouseMgmtUseCase.Controllers
{
    public class WarehouseManagementController : Controller
    {
        private readonly IWarehouseManagementWorkflowService _warehouseManagementWorkflowService;

        public WarehouseManagementController(IWarehouseManagementWorkflowService warehouseManagementWorkflowService)
        {
            _warehouseManagementWorkflowService = warehouseManagementWorkflowService;
        }

        public ViewResult Overview()
        {
            return View(new OutpostOverviewModel());
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase csvfile, Guid? outpostId)
        {
            // Verify that the user selected a file
            if (csvfile != null && csvfile.ContentLength > 0)
            {
                _warehouseManagementWorkflowService.ProcessWarehouseStockData(csvfile.InputStream, outpostId.Value);
            }
            else
            {
                TempData["invalidFile"] = Strings.InvalidFileSelectedForUpload;
            }

            return this.RedirectToAction(c => c.Overview());
        }
    }
}
