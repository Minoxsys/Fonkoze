using AutoMapper;
using Web.Helpers;

namespace Web.Bootstrap.Converters
{
    public class NullableDoubleToStringTypeConverter : TypeConverter<double?, string>
    {
        protected override string ConvertCore(double? source)
        {
            return NullableDoubleFormatter.DoubleToString(source);

        }
    }
    public class StringToNullableDoubleTypeConverter : TypeConverter<string, double?>
    {
        protected override double? ConvertCore(string source)
        {
            return NullableDoubleFormatter.StringToDouble(source);
        }
    }
}