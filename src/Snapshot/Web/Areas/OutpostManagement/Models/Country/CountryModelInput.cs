using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models.Country;
using Microsoft.Practices.Unity;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Areas.OutpostManagement;
//using Web.Areas.Employees.Models.Employee;
using Web.Validation.ValidDate;
using System.Globalization;
using Web.Models.Shared;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryModelInput
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}