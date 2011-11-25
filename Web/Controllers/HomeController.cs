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
using System.ComponentModel.DataAnnotations;

namespace Web.Controllers
{
    public class HomeController : Controller
    {

        //[Requires(Permissions = "Home.Index")]
        public ActionResult Index()
        {
            if (!this.ModelState.IsValid)
            {
                return new EmptyResult();
            }
            IndexModel listModel = new IndexModel();


            return View(listModel);
        }

       


    }
}