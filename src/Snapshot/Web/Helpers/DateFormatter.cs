using System;
using System.Globalization;

namespace Web.Helpers
{
    public class DateFormatter
    {
        public static string DateFormat = "dd-MMM-yyyy";

        public static IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

        public static string Placeholder = StringPlaceholder.Placeholder;

        public static string DateToShortString(DateTime dateTime)
        {
            
            return dateTime.ToString(DateFormat, FormatProvider);
        }
        
        public static string DateToShortString(DateTime? source)
        {
            if (source == null) return string.Empty;
            else return source.Value.ToString(DateFormat, FormatProvider);
        }

        public static DateTime StringToDate(string date)
        {
            DateTime result;
            if (string.IsNullOrWhiteSpace(date) || !DateTime.TryParseExact(date, DateFormat, FormatProvider, DateTimeStyles.None, out result)) 
                return DateTime.Today;
            return DateTime.ParseExact(date, DateFormat, FormatProvider);
        }

        public static DateTime? StringToDate(string source, bool nullable)
        {
            DateTime result;
            if (string.IsNullOrWhiteSpace(source) || source.Equals(Placeholder) || !DateTime.TryParseExact(source, DateFormat, FormatProvider, DateTimeStyles.None, out result) )
                return null;
            return DateTime.ParseExact(source, DateFormat,FormatProvider);
        }
    }
}