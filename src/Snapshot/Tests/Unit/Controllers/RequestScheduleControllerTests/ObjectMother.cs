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


        public IQueryService<RequestSchedule> queryServiceRequestSchedule;
        public ISaveOrUpdateCommand<RequestSchedule> saveCommandRequestSchedule;
        public IDeleteCommand<RequestSchedule> deleteCommandRequestSchedule;

        public RequestScheduleController controller;

        public Guid requestScheduleId;
        public RequestSchedule requestSchedule;
        public RequestScheduleInputModel inputModel;
        public IndexModel indexModel;

        public void Init_Controller_And_Mock_Services()
        {
            queryServiceRequestSchedule = MockRepository.GenerateMock<IQueryService<RequestSchedule>>();
            saveCommandRequestSchedule = MockRepository.GenerateMock<ISaveOrUpdateCommand<RequestSchedule>>();
            deleteCommandRequestSchedule = MockRepository.GenerateMock<IDeleteCommand<RequestSchedule>>();

            controller = new RequestScheduleController();
            controller.QueryServiceRequestSchedule = queryServiceRequestSchedule;
            controller.SaveCommandRequestSchedule = saveCommandRequestSchedule;
            controller.DeleteCommandRequestSchedule = deleteCommandRequestSchedule;
        }

        public void Init_Stub_Data()
        {
            requestScheduleId = Guid.NewGuid();
            requestSchedule = MockRepository.GeneratePartialMock<RequestSchedule>();
            requestSchedule.Stub(r => r.Id).Return(requestScheduleId);
            requestSchedule.FrequencyType = FREQUENCY_TYPE;
            requestSchedule.FrequencyValue = FREQUENCY_VALUE;
            requestSchedule.Reminders = new RequestReminder[] { new RequestReminder { PeriodType = PERIOD_TYPE, PeriodValue = PERIOD_VALUE } }.ToList();

            indexModel = new IndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "ScheduleName"
            };

            inputModel = new RequestScheduleInputModel
            {
                Id = requestScheduleId,
                ScheduleName = SCHEDULE_NAME,
                Basis = SCHEDULE_BASIS,
                FrequencyType = FREQUENCY_TYPE, 
                FrequencyValue = FREQUENCY_VALUE,
                StartOn = START_ON,
                Reminders = new RequestReminderInput[] { new RequestReminderInput { PeriodType = PERIOD_TYPE, PeriodValue = PERIOD_VALUE } }
            };
        }
    }
}
