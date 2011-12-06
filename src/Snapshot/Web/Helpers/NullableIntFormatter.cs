
namespace Web.Helpers
{
    public class NullableIntFormatter
    {
        public static string IntToString(int? source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            else return source.ToString();
        }

        public static int? StringToInt(string source)
        {
            int result = -1;
            if (string.IsNullOrEmpty(source) || !int.TryParse(source, out result))
            {
                return null;
            }

            return result;                
        }
    }
}