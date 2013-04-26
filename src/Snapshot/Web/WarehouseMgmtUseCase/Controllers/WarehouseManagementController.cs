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
                if (_warehouseManagementWorkflowService.ProcessWarehouseStockData(csvfile.InputStream, outpostId.Value))
                {
                    TempData["result"] = Strings.Upload_The_file_uploaded_succesfully;
                }
                else
                {
                    TempData["result"] = Strings.CSV_file_parsing_has_failed;
                }
            }
            else
            {
                TempData["result"] = Strings.InvalidFileSelectedForUpload;
            }

            return this.RedirectToAction(c => c.Overview());
        }
    }
}
