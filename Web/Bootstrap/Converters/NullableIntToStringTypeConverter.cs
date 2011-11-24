using AutoMapper;
using Web.Helpers;

namespace Web.Bootstrap.Converters
{
    public class NullableIntToStringTypeConverter : TypeConverter<int?, string>
    {
        protected override string ConvertCore(int? source)
        {
            return NullableIntFormatter.IntToString(source);

        }
    }
    public class StringToNullableIntTypeConverter : TypeConverter<string, int?>
    {
        protected override int? ConvertCore(string source)
        {
            return NullableIntFormatter.StringToInt(source);
        }
    }
}