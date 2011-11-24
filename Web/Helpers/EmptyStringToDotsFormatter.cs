using System.Reflection;

namespace Web.Helpers
{
    public class EmptyStringToDotsFormatter
    {
        /// <summary>
        /// Prepares a view model for display on screen (in a html page)
        /// Converts any empty string property into the placeholder
        /// </summary>
        /// <param name="model">The view model you wish to change</param>
        public static void ConvertModelForView(object model)
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            
            var placeholder = StringPlaceholder.Placeholder;

            foreach (var propertyInfo in properties)
            {
                var prop = propertyInfo.GetValue(model, null);
                if (prop == null || prop.ToString() == string.Empty)
                {
                    propertyInfo.SetValue(model, placeholder, null);
                }
            }
        }
    }
}