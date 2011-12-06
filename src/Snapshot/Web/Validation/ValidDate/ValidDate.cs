using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Web.Validation.ValidDate
{
    public class ValidDate : ValidationAttribute
    {
        public static string DateFormat = "dd/MM/yyyy";
        public static IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

        public ValidDate()
            : base("ValidDate")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }

            var stringDate = value as string;
            DateTime result = new DateTime();
            if (!DateTime.TryParseExact(stringDate, DateFormat, FormatProvider, DateTimeStyles.None, out result))
                return new ValidationResult("Invalid date: " + stringDate + " ! Check if the format is 'dd/MM/yyyy'");

            return ValidationResult.Success;
        }        
    }
}