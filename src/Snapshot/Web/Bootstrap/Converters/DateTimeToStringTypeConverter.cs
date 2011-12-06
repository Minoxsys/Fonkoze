using System;
using AutoMapper;
using Web.Helpers;

namespace Web.Bootstrap.Converters
{
    public class DateTimeToStringTypeConverter : TypeConverter<DateTime, string>
    {
        protected override string ConvertCore(DateTime source)
        {
            return DateFormatter.DateToShortString(source);

        }
    }
    public class StringToDateTimeTypeConverter : TypeConverter<string, DateTime>
    {
        protected override DateTime ConvertCore(string source)
        {
            return DateFormatter.StringToDate(source);
        }
    }
}