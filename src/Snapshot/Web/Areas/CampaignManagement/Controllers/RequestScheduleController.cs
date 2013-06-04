using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Shared;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Models.RequestSchedule;
using Core.Domain;
using Web.Security;
using Core.Security;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class RequestScheduleController : Controller
    {
        public const string ON_A_SCHEDULE_BASIS = "On a schedule";

        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Schedule> QueryServiceSchedule { get; set; }

        public ISaveOrUpdateCommand<Schedule> SaveCommandRequestSchedule { get; set; }

        public IDeleteCommand<Schedule> DeleteCommandRequestSchedule { get; set; }

        public IDeleteCommand<RequestReminder> DeleteCommandRequestReminder { get; set; }

        public IPermissionsService PermissionService { get; set; }
        private const String AUTOMATICSCHEDULE_ADD_PERMISSION = "AutomaticSchedule.Edit";
        private const String AUTOMATICSCHEDULE_DUPLICATE_PERMISSION = "AutomaticSchedule.Delete";

        [Requires(Permissions = "AutomaticSchedule.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(AUTOMATICSCHEDULE_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(AUTOMATICSCHEDULE_DUPLICATE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           

            return View();
        }

        private Client _client;
        private User _user;

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }

        [HttpGet]
        public JsonResult GetListOfRequestSchedules(IndexTableInputModel indexTableInputModel)
        {
            LoadUserAndClient();

            var pageSize = indexTableInputModel.limit.Value;
            var schedulesDataQuery = QueryServiceSchedule.Query().Where(s => s.Client.Id == _client.Id);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Schedule>>>()
            {
                { "Name-ASC", () => schedulesDataQuery.OrderBy(r => r.Name) },
                { "Name-DESC", () => schedulesDataQuery.OrderByDescending(r => r.Name) },
                { "ScheduleBasis-ASC", () => schedulesDataQuery.OrderBy(r => r.ScheduleBasis) },
                { "ScheduleBasis-DESC", () => schedulesDataQuery.OrderByDescending(r => r.ScheduleBasis) },
                { "Frequency-ASC", () => schedulesDataQuery.OrderBy(r => r.FrequencyType) },
                { "Frequency-DESC", () => schedulesDataQuery.OrderByDescending(r => r.FrequencyType) },
                { "Created-ASC", () => schedulesDataQuery.OrderBy(r => r.Created) },
                { "Created-DESC", () => schedulesDataQuery.OrderByDescending(r => r.Created) }
            };

            schedulesDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexTableInputModel.sort, indexTableInputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexTableInputModel.searchValue))
            {
                schedulesDataQuery = schedulesDataQuery.Where(it => it.Name.Contains(indexTableInputModel.searchValue));
            }

            var totalItems = schedulesDataQuery.Count();

            schedulesDataQuery = schedulesDataQuery
                .Take(pageSize)
                .Skip(indexTableInputModel.start.Value);

            var scheduleListOfReferenceModelsProjection = (from schedule in schedulesDataQuery.ToList()
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
            LoadUserAndClient();

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
            schedule.ScheduleBasis = ON_A_SCHEDULE_BASIS;
            //schedule.FrequencyType = inputModel.FrequencyType;
            schedule.FrequencyType = "Every " + inputModel.FrequencyValue + " day(s)";
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
            LoadUserAndClient();

            Schedule schedule = QueryServiceSchedule.Query().Where(s => s.Client == _client && s.Name.Equals(Name)).FirstOrDefault();

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
            Schedule schedule = new Schedule
            {
                Name = inputModel.Name,
                ScheduleBasis = ON_A_SCHEDULE_BASIS,
                //FrequencyType = inputModel.FrequencyType,
                FrequencyType = "Every " + inputModel.FrequencyValue + " day(s)",
                FrequencyValue = inputModel.FrequencyValue,
                StartOn = inputModel.StartOn,
                Client = _client
            };

            schedule.Reminders = new List<RequestReminder>();
            if (inputModel.Reminders != null)
            {
                foreach (RequestReminderInput reminder in inputModel.Reminders)
                {
                    schedule.AddReminder(new RequestReminder { PeriodType = reminder.PeriodType, PeriodValue = reminder.PeriodValue });
                }
            }

            SaveCommandRequestSchedule.Execute(schedule);

            return schedule;
        }
    }
}
