using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models.Country;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Areas.OutpostManagement;
using Web.Validation.ValidDate;
using System.Globalization;
using Web.Models.Shared;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryModel
    {

        [Required]
        public virtual string Name { get; set; }
        public virtual string ISOCode { get; set; }
        public virtual string PhonePrefix { get; set; }
        public virtual  ClientModelCountry Client { get; set; }
        public Guid Id { get; set; }
    }
}