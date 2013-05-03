using System;
using MvcContrib;
using System.Web;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.LocalizationResources;
using Web.WarehouseMgmtUseCase.Services;
using System.Text;
using Web.Services.StockUpdates;

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
// ReSharper disable Mvc.ViewNotResolved
            return View(new OutpostOverviewModel());
// ReSharper restore Mvc.ViewNotResolved
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase csvfile, Guid? outpostId)
        {
            // Verify that the user selected a file
            if (csvfile != null && csvfile.ContentLength > 0)
            {
                var stockUpdateResult = _warehouseManagementWorkflowService.ProcessWarehouseStockData(csvfile.InputStream, outpostId.Value);

                if (outpostId.HasValue && stockUpdateResult.Success)
                {
                    TempData["result"] = Strings.Upload_The_file_uploaded_succesfully;
                }
                else
                {
                    if (stockUpdateResult.FailedProducts == null)
                    {
                        TempData["result"] = Strings.CSV_file_parsing_has_failed;
                    }
                    else
                    {
                        TempData["result"] = Strings.CSV_file_failed_products + GetFailedProductsString(stockUpdateResult);
                    }
                }
            }
            else
            {
                TempData["result"] = Strings.InvalidFileSelectedForUpload;
            }

            return this.RedirectToAction(c => c.Overview());
        }

        private StringBuilder GetFailedProductsString(StockUpdateResult stockUpdateResult)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= stockUpdateResult.FailedProducts.Count; ++i)
            {
                if (i == 0)
                {
                    sb.Append(" ").Append(stockUpdateResult.FailedProducts[i].ProductCode);
                }
                else if (i == stockUpdateResult.FailedProducts.Count)
                {
                    sb.Append(".");
                }
                else
                {
                    sb.Append(", ").Append(stockUpdateResult.FailedProducts[i].ProductCode);
                }
            }

            return sb;
        }
    }
}
