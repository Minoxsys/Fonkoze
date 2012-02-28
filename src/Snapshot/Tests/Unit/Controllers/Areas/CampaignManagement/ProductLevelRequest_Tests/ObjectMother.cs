using System;
using System.Linq;
using Web.Areas.CampaignManagement.Controllers;
using AutofacContrib.Moq;
using Moq;
using Core.Domain;
using Domain;
using Autofac;
using MvcContrib.TestHelper.Fakes;
using System.Collections.Generic;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    public class ObjectMother
    {
        const string FAKE_USERNAME = "fake.username";

        internal ProductLevelRequestController controller;

        internal AutoMock autoMock;
        private Guid clientId = Guid.NewGuid();
        private Mock<User> userMock;
        public Mock<Client> clientMock;

        internal void Init()
        {
            autoMock = AutoMock.GetLoose();

            InitializeController();
            StubUserAndItsClient();
        }

        internal void StubUserAndItsClient()
        {
            var loadClient = Mock.Get(this.controller.LoadClient);
            var queryUser = Mock.Get(this.controller.QueryUsers);

            this.clientMock = new Mock<Client>();
            clientMock.Setup(c => c.Id).Returns(this.clientId);
            clientMock.Setup(c => c.Name).Returns("minoxsys");

            this.userMock = new Mock<User>();
            userMock.Setup(c => c.Id).Returns(Guid.NewGuid());
            userMock.Setup(c => c.ClientId).Returns(clientMock.Object.Id);
            userMock.Setup(c => c.UserName).Returns(FAKE_USERNAME);
            userMock.Setup(c => c.Password).Returns("asdf");

            loadClient.Setup(c => c.Load(this.clientId)).Returns(clientMock.Object);
            queryUser.Setup(c => c.Query()).Returns(new[] { userMock.Object }.AsQueryable());

            controller.LoadClient = loadClient.Object;
            controller.QueryUsers = queryUser.Object;
        }

        private void InitializeController()
        {
            controller = new ProductLevelRequestController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            autoMock.Container.InjectUnsetProperties(controller);
        }

        internal void VerifyUserAndClientExpectations()
        {
            var queryUser = Mock.Get(this.controller.QueryUsers);
            var loadClient = Mock.Get(this.controller.LoadClient);

            queryUser.Verify(call => call.Query());
            loadClient.Verify(call => call.Load(this.clientId));
        }




        internal void VerifySchedulesQueried()
        {
            var querySchedules = Mock.Get(this.controller.QueryServiceSchedule);

            querySchedules.Verify(call => call.Query());
        }

        internal void StubSchedulesData()
        {
            
            var querySchedules = Mock.Get(this.controller.QueryServiceSchedule);

            querySchedules.Setup(c => c.Query()).Returns(ListOfSchedules());
        }

        private IQueryable<Schedule> ListOfSchedules()
        {
            var schedules = new List<Schedule>();

            for (int i = 0; i < 10; i++)
            {
                schedules.Add(ScheduleMock(i));
            }

            return schedules.AsQueryable();
        }

        private Schedule ScheduleMock(int i)
        {
            var schedule = new Mock<Schedule>();

            schedule.SetupGet(x => x.Id).Returns(Guid.NewGuid());
            schedule.SetupGet(x => x.Name).Returns("Schedule " + i);
            schedule.SetupGet(x => x.ScheduleBasis).Returns("On a schedule");
            schedule.SetupGet(x => x.FrequencyType).Returns("Daily");
            schedule.SetupGet(x => x.FrequencyValue).Returns(2);

            schedule.Setup(x => x.Reminders).Returns(Reminders(schedule.Object));

            return schedule.Object;
        }

        private  IList<RequestReminder> Reminders(Schedule schedule)
        {
            var reminder = new Mock<RequestReminder>();

            reminder.SetupGet(x => x.Id).Returns(Guid.NewGuid());
            reminder.SetupGet(x => x.PeriodType).Returns("Day");
            reminder.SetupGet(x => x.PeriodValue).Returns(5);
            reminder.SetupGet(x => x.Schedule).Returns(schedule);
            reminder.SetupGet(x => x.ByUser).Returns(userMock.Object);

            return new List<RequestReminder>{ reminder.Object};
        }
    }
}
