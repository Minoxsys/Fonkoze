using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Web.Models.Home;
using Core.Persistence;
using AutoMapper;
using Web.Bootstrap.Converters;
using Web.Models.Shared;
using Web.Security;
using System.ComponentModel.DataAnnotations;

namespace Web.Controllers
{
    public class HomeController : Controller
    {

        [Requires(Permissions = "Home.Index")]
        public ActionResult Index()
        {
            if (!this.ModelState.IsValid)
            {
                return new EmptyResult();
            }
            Web.Models.Home.IndexModel listModel = new Web.Models.Home.IndexModel();


            return View(listModel);
        }




    }
}