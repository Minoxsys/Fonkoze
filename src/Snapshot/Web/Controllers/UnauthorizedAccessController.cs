using System.Web.Mvc;
using Web.Models.UnauthorizedAccess;

namespace Web.Controllers
{
    public class UnauthorizedAccessController : Controller
    {
        public ActionResult Index()
        {
            return View(new Index());
        }
    }
}
