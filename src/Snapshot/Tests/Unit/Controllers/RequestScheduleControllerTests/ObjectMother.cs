using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Models.Shared;
using Web.Areas.CampaignManagement.Controllers;
using Web.Areas.CampaignManagement.Models.RequestSchedule;

namespace Tests.Unit.Controllers.RequestScheduleControllerTests
{
    public class ObjectMother
    {
        public const string SCHEDULE_NAME = "Schedule New";
        public const string SCHEDULE_BASIS = "On a schedule";
        public const string FREQUENCY_TYPE = "Daily";
        public const int FREQUENCY_VALUE = 5;
        public const int START_ON = 3;
        public const string PERIOD_TYPE = "hours";
        public const int PERIOD_VALUE = 24;


        public IQueryService<Schedule> queryServicetSchedule;
        public ISaveOrUpdateCommand<Schedule> saveCommandSchedule;
        public IDeleteCommand<Schedule> deleteCommandRequestSchedule;
        public IDeleteCommand<RequestReminder> deleteCommandRequestReminder;

        public RequestScheduleController controller;

        public Guid scheduleId;
        public Schedule schedule;
        public RequestReminder reminder;
        public RequestScheduleInputModel inputModel;
        public IndexModel indexModel;

        public void Init_Controller_And_Mock_Services()
        {
            queryServicetSchedule = MockRepository.GenerateMock<IQueryService<Schedule>>();
            saveCommandSchedule = MockRepository.GenerateMock<ISaveOrUpdateCommand<Schedule>>();
            deleteCommandRequestSchedule = MockRepository.GenerateMock<IDeleteCommand<Schedule>>();
            deleteCommandRequestReminder = MockRepository.GenerateMock<IDeleteCommand<RequestReminder>>();

            controller = new RequestScheduleController();
            controller.QueryServiceSchedule = queryServicetSchedule;
            controller.SaveCommandRequestSchedule = saveCommandSchedule;
            controller.DeleteCommandRequestSchedule = deleteCommandRequestSchedule;
            controller.DeleteCommandRequestReminder = deleteCommandRequestReminder;
        }

        public void Init_Stub_Data()
        {
            reminder = new RequestReminder { PeriodType = PERIOD_TYPE, PeriodValue = PERIOD_VALUE };

            scheduleId = Guid.NewGuid();
            schedule = MockRepository.GeneratePartialMock<Schedule>();
            schedule.Stub(r => r.Id).Return(scheduleId);
            schedule.FrequencyType = FREQUENCY_TYPE;
            schedule.FrequencyValue = FREQUENCY_VALUE;
            schedule.Name = SCHEDULE_NAME;
            schedule.Reminders = new RequestReminder[] { reminder }.ToList();

            indexModel = new IndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };

            inputModel = new RequestScheduleInputModel
            {
                Id = scheduleId,
                Name = SCHEDULE_NAME,
                Basis = SCHEDULE_BASIS,
                FrequencyType = FREQUENCY_TYPE, 
                FrequencyValue = FREQUENCY_VALUE,
                StartOn = START_ON,
                Reminders = new RequestReminderInput[] { new RequestReminderInput { PeriodType = PERIOD_TYPE, PeriodValue = PERIOD_VALUE } }
            };
        }
    }
}
