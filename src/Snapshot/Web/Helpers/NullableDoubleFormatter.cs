
namespace Web.Helpers
{
    public class NullableDoubleFormatter
    {

        public static string DoubleToString(double? source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            else return source.ToString();
        }

        public static double? StringToDouble(string source)
        {
            double result = -1.0; 
            if (string.IsNullOrEmpty(source) || !double.TryParse(source, out result))
            {
                return null;
            }
            return result;
        }
    }
}