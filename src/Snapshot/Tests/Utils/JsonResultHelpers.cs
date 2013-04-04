using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Tests.Utils
{
    /// <summary>
    /// Contains extension methods that help unit testign of JsonResult in ASP.NET MVC.
    /// </summary>
    public static class JsonResultExtensions
    {
        /// <summary>
        /// Extracts the property value specified by name from the current JsonResult instance.
        /// </summary>
        /// <typeparam name="T">The result type to be returned.</typeparam>
        /// <param name="jsonResult">The JsonResult instance.</param>
        /// <param name="propertyName">The name of the property whose value is to be returned.</param>
        /// <returns>The value hold by the poperty specified.</returns>
        public static T GetValueFromJsonResult<T>(this JsonResult jsonResult, string propertyName)
        {
            var property = jsonResult.Data.GetType().GetProperties().FirstOrDefault(p => String.CompareOrdinal(p.Name, propertyName) == 0);

            if (property == null)
            {
                throw new ArgumentException("propertyName not found", "propertyName");
            }

            return (T)property.GetValue(jsonResult.Data, null);
        }

        /// <summary>
        /// Extracts the value from the model encapsulated by the current JsonResult instance.
        /// </summary>
        /// <typeparam name="TModel">The type for the Model in the Data property.</typeparam>
        /// <typeparam name="TPropertyType">The result type to be returned.</typeparam>
        /// <param name="jsonResult">The JsonResult instance.</param>
        /// <param name="propertyName">A lambda expression that specifies the property to extract.</param>
        /// <returns>The value converted to the type specified by TPropertyType type parameter for the given property.</returns>
        public static TPropertyType GetValueFromJsonResultForModel<TModel, TPropertyType>(this JsonResult jsonResult,
                                                                                          Expression<Func<TModel, TPropertyType>> propertyName)
        {
            var member = ((MemberExpression)propertyName.Body).Member.Name;
            var property =
                jsonResult.Data.GetType()
                          .GetProperties()
                          .FirstOrDefault(p => String.CompareOrdinal(p.Name, member) == 0);

            if (property == null)
            {
                throw new ArgumentException("propertyName not found", member);
            }

            return (TPropertyType)property.GetValue(jsonResult.Data, null);
        }
    }
}