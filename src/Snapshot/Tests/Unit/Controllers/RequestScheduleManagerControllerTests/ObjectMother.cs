using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.RequestScheduleManagerControllerTests
{
    public class ObjectMother
    {

        public IQueryService<RequestSchedule> queryServiceRequestShcedule;

        public RequestScheduleManagerController controller;

        public Guid requestScheduleId;
        public RequestSchedule requestSchedule;
        public IndexModel indexModel { get; set; }

        public void Init_Controller_And_Mock_Services()
        {
            queryServiceRequestShcedule = MockRepository.GenerateMock<IQueryService<RequestSchedule>>();
            controller = new RequestScheduleManagerController();
            controller.QueryServiceRequestSchedule = queryServiceRequestShcedule;
        }

        public void Init_Stub_Data()
        {
            requestScheduleId = Guid.NewGuid();
            requestSchedule = MockRepository.GeneratePartialMock<RequestSchedule>();
            requestSchedule.Stub(r => r.Id).Return(requestScheduleId);
            requestSchedule.Frequency = new ScheduleFrequency { FrequencyType = "Daily", FrequencyValue = 3 };
            requestSchedule.Reminders = new RequestReminder[] { new RequestReminder { PeriodType = "hours", PeriodValue = 24 } }.ToList();

            indexModel = new IndexModel()
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "ScheduleName"
            };
        }
    }
}
