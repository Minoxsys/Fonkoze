using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Microsoft.Practices.Unity;
using Web.Models.Home;
using Core.Persistence;
using AutoMapper;
using Web.Bootstrap.Converters;
using Web.Models.Shared;
using Web.Security;

namespace Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {

        [Requires(Permissions = "Home.Index")]
        public ActionResult Index()
        {
            IndexModel listModel = new IndexModel();


            return View(listModel);
        }

       


    }
}