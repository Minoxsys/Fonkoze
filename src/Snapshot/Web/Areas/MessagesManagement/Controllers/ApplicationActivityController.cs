using Core.Persistence;
using Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.MessagesManagement.Models.ApplicationActivity;
using Web.Models.Shared;
using Web.Security;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class ApplicationActivityController : Controller
    {
        private readonly IQueryService<ApplicationActivity> _activityQueryService;

        public ApplicationActivityController(IQueryService<ApplicationActivity> activityQueryService)
        {
            _activityQueryService = activityQueryService;
        }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetActivityItems(IndexTableInputModel inputModel)
        {
            Debug.Assert(inputModel.limit != null, "inputModel.limit != null");
            var pageSize = inputModel.limit.Value;
            var rawDataQuery = _activityQueryService.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<ApplicationActivity>>>
                {
                    {"Message-ASC", () => rawDataQuery.OrderBy(c => c.Message)},
                    {"Message-DESC", () => rawDataQuery.OrderByDescending(c => c.Message)},
                    {"Created-ASC", () => rawDataQuery.OrderBy(c => c.Created)},
                    {"Created-DESC", () => rawDataQuery.OrderByDescending(c => c.Created)},
                };

            rawDataQuery = orderByColumnDirection[String.Format("{0}-{1}", inputModel.sort, inputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(inputModel.searchValue))
                rawDataQuery = rawDataQuery.Where(it => it.Message.Contains(inputModel.searchValue));

            var totalItems = rawDataQuery.Count();

            Debug.Assert(inputModel.start != null, "inputModel.start != null");
            rawDataQuery = rawDataQuery
                .Take(pageSize)
                .Skip(inputModel.start.Value);

            var otherActivityModelListProjection = (from activity in rawDataQuery.ToList()
                                                    select new ApplicationActivityViewModel
                                                        {
                                                            Created = activity.Created.HasValue ? activity.Created.Value.ToString("dd-MMM-yyyy, HH:mm") : "-",
                                                            Message = activity.Message,
                                                        }).ToArray();


            return Json(new
                {
                    Activities = otherActivityModelListProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }
    }
}
