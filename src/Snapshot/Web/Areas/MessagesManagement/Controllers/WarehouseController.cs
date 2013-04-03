using Domain.Enums;
using System.Web.Mvc;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Security;
using Web.Services;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class WarehouseController : Controller
    {
        public IRawSmsMeesageQueryHelpersService SmsQueryService { get; set; }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetMessagesFromWarehouse(MessagesIndexModel indexModel)
        {
            return Json(SmsQueryService.GetMessagesFromOutpost(indexModel, OutpostType.Warehouse), JsonRequestBehavior.AllowGet);
        }
    }
}
