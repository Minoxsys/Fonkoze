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

        public IQueryService<Schedule> QueryServiceSchedule { get; set; }

        public ISaveOrUpdateCommand<Schedule> SaveCommandRequestSchedule { get; set; }

        public IDeleteCommand<Schedule> DeleteCommandRequestSchedule { get; set; }

        public IDeleteCommand<RequestReminder> DeleteCommandRequestReminder { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetListOfRequestSchedules(IndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var schedulesDataQyery = QueryServiceSchedule.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Schedule>>>()
            {
                { "Name-ASC", () => schedulesDataQyery.OrderBy(r => r.Name) },
                { "Name-DESC", () => schedulesDataQyery.OrderByDescending(r => r.Name) },
                { "ScheduleBasis-ASC", () => schedulesDataQyery.OrderBy(r => r.ScheduleBasis) },
                { "ScheduleBasis-DESC", () => schedulesDataQyery.OrderByDescending(r => r.ScheduleBasis) },
                { "Frequency-ASC", () => schedulesDataQyery.OrderBy(r => r.FrequencyType) },
                { "Frequency-DESC", () => schedulesDataQyery.OrderByDescending(r => r.FrequencyType) },
                { "Created-ASC", () => schedulesDataQyery.OrderBy(r => r.Created) },
                { "Created-DESC", () => schedulesDataQyery.OrderByDescending(r => r.Created) }
            };

            schedulesDataQyery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                schedulesDataQyery = schedulesDataQyery.Where(it => it.Name.Contains(indexModel.searchValue));
            }

            var totalItems = schedulesDataQyery.Count();

            schedulesDataQyery = schedulesDataQyery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var scheduleListOfReferenceModelsProjection = (from schedule in schedulesDataQyery.ToList()
                                                           select new RequestScheduleReferenceModel
                                                       {
                                                           Id = schedule.Id,
                                                           Name = schedule.Name,
                                                           FrequencyType = schedule.FrequencyType ?? "-",
                                                           FrequencyValue = schedule.FrequencyValue,
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
            Schedule request = CreateRequestScheduleFromRequestScheduleInputModel(inputModel);

            return Json(new JsonActionResponse() { Status = "Success", Message = string.Format("Request {0} has been saved.", request.Name) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(RequestScheduleInputModel inputModel)
        {
            if (inputModel.Id == Guid.Empty)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply a scheduleId in order to edit the schedule." });
            }

            Schedule schedule = QueryServiceSchedule.Load(inputModel.Id);

            if (schedule == null)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply the scheduleId of a schedule that exists in the DB in order to edit it." });
            }

            schedule.Name = inputModel.Name;
            schedule.ScheduleBasis = inputModel.Basis;
            schedule.FrequencyType = inputModel.FrequencyType;
            schedule.FrequencyValue = inputModel.FrequencyValue;
            schedule.StartOn = inputModel.StartOn;

            RemoveRemindersFromSchedule(schedule);

            foreach (RequestReminderInput reminder in inputModel.Reminders)
            {
                schedule.AddReminder(new RequestReminder { PeriodType = reminder.PeriodType, PeriodValue = reminder.PeriodValue });
            }

            SaveCommandRequestSchedule.Execute(schedule);

            return Json(new JsonActionResponse() { Status = "Success", Message = string.Format("Request {0} has been updated.", schedule.Name) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(Guid? scheduleId)
        {
            if (!scheduleId.HasValue)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply a scheduleId in order to remove the schedule." });
            }

            Schedule schedule = QueryServiceSchedule.Load(scheduleId.Value);

            if (schedule == null)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply the scheduleId of a role that exists in the DB in order to remove it." });
            }

            RemoveRemindersFromSchedule(schedule);
            DeleteCommandRequestSchedule.Execute(schedule);

            return Json(new JsonActionResponse() { Status = "Success", Message = "Schedule " + schedule.Name + " was removed." }); ;
        }

        [HttpPost]
        public JsonResult DoesScheduleExist(string Name)
        {
            Schedule schedule = QueryServiceSchedule.Query().Where(s => s.Name.Equals(Name)).FirstOrDefault();

            if (schedule == null)
            {
                return Json(new JsonActionResponse() { Status = "NotFound", Message = "Schedule " + Name + " was not found in the DB." });
            }
            else
            {
                return Json(new JsonActionResponse() { Status = "Success", Message = "There is a schedule with the name " + Name + " in the DB." });
            }
        }

        private void RemoveRemindersFromSchedule(Schedule schedule)
        {
            while (schedule.Reminders.Count > 0)
            {
                RequestReminder reminder = schedule.Reminders[0];
                schedule.Reminders.Remove(reminder);
                DeleteCommandRequestReminder.Execute(reminder);
            }
        }

        private Schedule CreateRequestScheduleFromRequestScheduleInputModel(RequestScheduleInputModel inputModel)
        {
            Schedule request = new Schedule
            {
                Name = inputModel.Name,
                ScheduleBasis = inputModel.Basis,
                FrequencyType = inputModel.FrequencyType,
                FrequencyValue = inputModel.FrequencyValue,
                StartOn = inputModel.StartOn
            };

            request.Reminders = new List<RequestReminder>();
            if (inputModel.Reminders != null)
            {
                foreach (RequestReminderInput reminder in inputModel.Reminders)
                {
                    request.AddReminder(new RequestReminder { PeriodType = reminder.PeriodType, PeriodValue = reminder.PeriodValue });
                }
            }

            SaveCommandRequestSchedule.Execute(request);

            return request;
        }
    }
}
