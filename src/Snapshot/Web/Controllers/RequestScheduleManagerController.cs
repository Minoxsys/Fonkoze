using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Shared;
using Core.Persistence;
using Domain;
using Web.Models.RequestScheduleManager;

namespace Web.Controllers
{
    public class RequestScheduleManagerController : Controller
    {

        public IQueryService<RequestSchedule> QueryServiceRequestSchedule { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetListOfRequestSchedules(IndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var requestSchedulesDataQyery = QueryServiceRequestSchedule.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<RequestSchedule>>>()
            {
                { "ScheduleName-ASC", () => requestSchedulesDataQyery.OrderBy(r => r.ScheduleName) },
                { "ScheduleName-DESC", () => requestSchedulesDataQyery.OrderByDescending(r => r.ScheduleName) },
                { "ScheduleBasis-ASC", () => requestSchedulesDataQyery.OrderBy(r => r.ScheduleBasis) },
                { "ScheduleBasis-DESC", () => requestSchedulesDataQyery.OrderByDescending(r => r.ScheduleBasis) },
                { "Frequency-ASC", () => requestSchedulesDataQyery.OrderBy(r => r.Frequency.FrequencyType) },
                { "Frequency-DESC", () => requestSchedulesDataQyery.OrderByDescending(r => r.Frequency.FrequencyType) },
                { "Created-ASC", () => requestSchedulesDataQyery.OrderBy(r => r.Created) },
                { "Created-DESC", () => requestSchedulesDataQyery.OrderByDescending(r => r.Created) }
            };

            requestSchedulesDataQyery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                requestSchedulesDataQyery = requestSchedulesDataQyery.Where(it => it.ScheduleName.Contains(indexModel.searchValue));
            }

            var totalItems = requestSchedulesDataQyery.Count();

            requestSchedulesDataQyery = requestSchedulesDataQyery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var scheduleListOfReferenceModelsProjection = (from schedule in requestSchedulesDataQyery.ToList()
                                                           select new RequestScheduleReferenceModel
                                                       {
                                                           Id = schedule.Id,
                                                           ScheduleName = schedule.ScheduleName,
                                                           Frequency = schedule.Frequency.FrequencyType,
                                                           Reminders = schedule.Reminders
                                                       }).ToArray();

            return Json(new RequestScheduleListForJsonOutput
            {
                RequestSchedules = scheduleListOfReferenceModelsProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
