using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public static class FormattingExtensions
    {
        public static string ToDigitFormat(this double number)
        {
            return String.Format("{0:0.####}",number);
        } 
    }
}