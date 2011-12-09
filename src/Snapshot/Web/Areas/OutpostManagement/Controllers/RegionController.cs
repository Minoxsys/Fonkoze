using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
//using Web.Areas.OnBoarding.Models.Candidate;
//using Web.Areas.OutpostManagement.Models.District;
using Microsoft.Practices.Unity;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Areas.OutpostManagement.Models;
//using Web.Areas.Employees.Models.Employee;
using Web.Validation.ValidDate;
using System.Globalization;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class RegionController : Controller
    {
        //
        // GET: /OnBoarding/Region/

        public ActionResult Index()
        {
            return View();
        }

    }
}
