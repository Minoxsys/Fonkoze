using System;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Models.Shared;

namespace Web.CustomFilters
{
    public class ApplicationActivityFilterAttribute: ActionFilterAttribute
    {
        public ISaveOrUpdateCommand<ApplicationActivity> ApplicationActivitySaveCommand { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var jsonResult = filterContext.Result as JsonResult;
            if (jsonResult != null)
            {
                var jsonActionResponse = jsonResult.Data as JsonActionResponse;
                if (jsonActionResponse != null)
                {
                    if (String.Compare(jsonActionResponse.Status, "success", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var resultMessage = jsonActionResponse.Message;
                        ApplicationActivitySaveCommand.Execute(new ApplicationActivity {Message = resultMessage});
                    }
                }
            }
        }
    }
}