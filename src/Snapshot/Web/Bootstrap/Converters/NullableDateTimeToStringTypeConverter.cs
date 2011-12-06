using System;
using AutoMapper;
using Web.Helpers;

namespace Web.Bootstrap.Converters
{
    public class NullableDateTimeToStringTypeConverter : TypeConverter<DateTime?, string>
    {
        protected override string ConvertCore(DateTime? source)
        {
            return DateFormatter.DateToShortString(source);
        }
    }
    public class StringToNullableDateTimeTypeConverter : TypeConverter<string, DateTime?>
    {
        protected override DateTime? ConvertCore(string source)
        {
            return DateFormatter.StringToDate(source, true);
        }
    }
}