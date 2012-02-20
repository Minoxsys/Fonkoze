using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Shared;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Models.RequestSchedule;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class RequestScheduleController : Controller
    {

        public IQueryService<RequestSchedule> QueryServiceRequestSchedule { get; set; }

        public ISaveOrUpdateCommand<RequestSchedule> SaveCommandRequestSchedule { get; set; }

        public IDeleteCommand<RequestSchedule> DeleteCommandRequestSchedule { get; set; }

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
                { "Frequency-ASC", () => requestSchedulesDataQyery.OrderBy(r => r.FrequencyType) },
                { "Frequency-DESC", () => requestSchedulesDataQyery.OrderByDescending(r => r.FrequencyType) },
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
                                                           Frequency = schedule.FrequencyType ?? "-",
                                                           Basis = schedule.ScheduleBasis,
                                                           CreationDate = schedule.Created.Value.ToString("dd-MMM-yyyy"),
                                                           Reminders = (from reminder in schedule.Reminders.ToList()
                                                                        select new RequestReminderOutputModel {
                                                                            PeriodType = reminder.PeriodType,
                                                                            PeriodValue = reminder.PeriodValue
                                                                        }).ToList()
                                                       }).ToArray();

            return Json(new RequestScheduleListForJsonOutput
            {
                RequestSchedules = scheduleListOfReferenceModelsProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(RequestScheduleInputModel inputModel)
        {
            RequestSchedule request = CreateRequestScheduleFromRequestScheduleInputModel(inputModel);

            return Json(new JsonActionResponse() { Status = "Success", Message = string.Format("Request {0} has been saved.", request.ScheduleName) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(RequestScheduleInputModel inputModel)
        {
            if (inputModel.Id == Guid.Empty)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply a scheduleId in order to edit the schedule." });
            }

            RequestSchedule request = QueryServiceRequestSchedule.Load(inputModel.Id);

            if (request == null)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply the scheduleId of a schedule that exists in the DB in order to edit it." });
            }

            request.ScheduleName = inputModel.ScheduleName;
            request.ScheduleBasis = inputModel.Basis;
            request.FrequencyType = inputModel.FrequencyType;
            request.FrequencyValue = inputModel.FrequencyValue;
            request.StartOn = inputModel.StartOn;

            List<RequestReminder> remindersToRemove = new List<RequestReminder>();

            //foreach (RequestReminder reminder in request.Reminders)
            //{
            //    if (!inputModel.Reminders.Contains(reminder))
            //    {
            //        remindersToRemove.Add(reminder);
            //    }
            //}

            //foreach (RequestReminder reminder in remindersToRemove)
            //{
            //    request.Reminders.Remove(reminder);
            //}

            //foreach (RequestReminder reminder in inputModel.Reminders)
            //{
            //    request.Reminders.Add(reminder);
            //}

            SaveCommandRequestSchedule.Execute(request);

            return Json(new JsonActionResponse() { Status = "Success", Message = string.Format("Request {0} has been updated.", request.ScheduleName) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(Guid? scheduleId)
        {
            if (!scheduleId.HasValue)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply a scheduleId in order to remove the schedule." });
            }

            RequestSchedule schedule = QueryServiceRequestSchedule.Load(scheduleId.Value);

            if (schedule == null)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply the scheduleId of a role that exists in the DB in order to remove it." });
            }

            DeleteCommandRequestSchedule.Execute(schedule);

            return Json(new JsonActionResponse() { Status = "Success", Message = "Schedule " + schedule.ScheduleName + " was removed." }); ;
        }

        private RequestSchedule CreateRequestScheduleFromRequestScheduleInputModel(RequestScheduleInputModel inputModel)
        {
            RequestSchedule request = new RequestSchedule
            {
                ScheduleName = inputModel.ScheduleName,
                ScheduleBasis = inputModel.Basis,
                FrequencyType = inputModel.FrequencyType,
                FrequencyValue = inputModel.FrequencyValue,
                StartOn = inputModel.StartOn
            };

            request.Reminders = new List<RequestReminder>();

            foreach (RequestReminderInput reminder in inputModel.Reminders)
            {
                request.AddReminder(new RequestReminder { PeriodType = reminder.PeriodType, PeriodValue = reminder.PeriodValue });
            }

            SaveCommandRequestSchedule.Execute(request);

            return request;
        }
    }
}
