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
using Core.Domain;
using MvcContrib.TestHelper.Fakes;

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
        const string USERNAME = "admin";


        public IQueryService<User> queryServiceUsers { get; set; }
        public IQueryService<Client> queryServiceClients { get; set; }

        public IQueryService<Schedule> queryServicetSchedule;
        public ISaveOrUpdateCommand<Schedule> saveCommandSchedule;
        public IDeleteCommand<Schedule> deleteCommandRequestSchedule;
        public IDeleteCommand<RequestReminder> deleteCommandRequestReminder;

        public RequestScheduleController controller;

        public Guid clientId;
        public Guid scheduleForClientId;
        public Guid theOtherClientId;
        public Guid scheduleForOtherClientId;
        public Guid userId;

        public Client client;
        public Client otherClient;
        public User user;
        public Schedule scheduleForClient;
        public Schedule scheduleForOtherClient;
        public RequestReminder reminder;
        public RequestScheduleInputModel inputModel;
        public IndexModel indexModel;

        public void Init_Controller_And_Mock_Services()
        {
            queryServiceUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryServiceClients = MockRepository.GenerateMock<IQueryService<Client>>();

            queryServicetSchedule = MockRepository.GenerateMock<IQueryService<Schedule>>();
            saveCommandSchedule = MockRepository.GenerateMock<ISaveOrUpdateCommand<Schedule>>();
            deleteCommandRequestSchedule = MockRepository.GenerateMock<IDeleteCommand<Schedule>>();
            deleteCommandRequestReminder = MockRepository.GenerateMock<IDeleteCommand<RequestReminder>>();

            controller = new RequestScheduleController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);
            controller.QueryUsers = queryServiceUsers;
            controller.QueryClients = queryServiceClients;
            controller.QueryServiceSchedule = queryServicetSchedule;
            controller.SaveCommandRequestSchedule = saveCommandSchedule;
            controller.DeleteCommandRequestSchedule = deleteCommandRequestSchedule;
            controller.DeleteCommandRequestReminder = deleteCommandRequestReminder;
        }

        public void Init_Stub_Data()
        {
            clientId = Guid.NewGuid();

            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(u => u.Id).Return(userId);
            user.UserName = USERNAME;
            user.ClientId = clientId;

            reminder = new RequestReminder { PeriodType = PERIOD_TYPE, PeriodValue = PERIOD_VALUE };

            scheduleForClientId = Guid.NewGuid();
            scheduleForClient = MockRepository.GeneratePartialMock<Schedule>();
            scheduleForClient.Stub(r => r.Id).Return(scheduleForClientId);
            scheduleForClient.FrequencyType = FREQUENCY_TYPE;
            scheduleForClient.FrequencyValue = FREQUENCY_VALUE;
            scheduleForClient.Name = SCHEDULE_NAME;
            scheduleForClient.Reminders = new RequestReminder[] { reminder }.ToList();
            scheduleForClient.Client = client;

            theOtherClientId = Guid.NewGuid();
            otherClient = MockRepository.GeneratePartialMock<Client>();
            otherClient.Stub(c => c.Id).Return(theOtherClientId);

            scheduleForOtherClientId = new Guid();
            scheduleForOtherClient = MockRepository.GeneratePartialMock<Schedule>();
            scheduleForOtherClient.Stub(s => s.Id).Return(scheduleForOtherClientId);
            scheduleForOtherClient.Client = otherClient;

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
                Id = scheduleForClientId,
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
