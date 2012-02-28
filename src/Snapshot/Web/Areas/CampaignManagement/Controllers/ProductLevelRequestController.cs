using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class ProductLevelRequestController : Controller
    {
        private Core.Domain.User _user;
        private Client _client;

        public IQueryService<Client> LoadClient { get; set; }

        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Schedule> QueryServiceSchedule { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetSchedules()
        {
            LoadUserAndClient();

            var schedulesData = QueryServiceSchedule.Query().ToList();

            var schedules = schedulesData.Select(schedule =>
                new ScheduleModel
                {
                    Basis = schedule.ScheduleBasis,
                    Frequency = schedule.FrequencyType??string.Empty,
                    ScheduleName = schedule.Name,
                    Id = schedule.Id.ToString(),
                    Reminders = schedule.Reminders.Select(reminder => new RequestReminderModel
                    {
                        PeriodType = reminder.PeriodType,
                        PeriodValue = reminder.PeriodValue
                    }).ToArray()
                }).ToArray();

            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = LoadClient.Load(clientId);
        }


    }
}
