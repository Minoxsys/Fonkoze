using Domain.Enums;
using System.Web.Mvc;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Security;
using Web.Services;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class SellerController : Controller
    {
        public IRawSmsMeesageQueryHelpersService SmsQueryService { get; set; }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetMessagesFromSeller(MessagesIndexModel indexModel)
        {
            return Json(SmsQueryService.GetMessagesFromOutpost(indexModel, OutpostType.Seller), JsonRequestBehavior.AllowGet);
        }
    }
}