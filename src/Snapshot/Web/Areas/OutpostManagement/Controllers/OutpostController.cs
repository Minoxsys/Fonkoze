using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
using Microsoft.Practices.Unity;
using AutoMapper;
//using Web.Areas.OutpostManagement.Services;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
//using Web.Areas.OffBoarding.Models;
//using Web.Areas.Employees.Models.Employee;
using Web.Validation.ValidDate;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class OutpostController : Controller
    {
        //
        // GET: /OnBoarding/Outpost/

        public ActionResult Index()
        {
            return View();
        }

    }
}
