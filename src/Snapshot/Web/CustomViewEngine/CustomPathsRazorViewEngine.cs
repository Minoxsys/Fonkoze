using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.CustomViewEngine
{
    public class CustomPathsRazorViewEngine : RazorViewEngine
    {
        public void AddViewLocationFormat(string paths)
        {
            var existingPaths = new List<string>(ViewLocationFormats) {paths};
            ViewLocationFormats = existingPaths.ToArray();
        }

        public void AddPartialViewLocationFormat(string paths)
        {
            var existingPaths = new List<string>(PartialViewLocationFormats) {paths};
            PartialViewLocationFormats = existingPaths.ToArray();
        }
    }
}