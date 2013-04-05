using System.Diagnostics;
using Core.Domain;
using Core.Persistence;
using Domain;
using Domain.Enums;
using MvcContrib.TestHelper.Fakes;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Controllers;
using Web.Models.Alerts;

namespace Tests.Unit.Controllers.AlertsController_Tests
{
    public class ObjectMother
    {
        public AlertsController controller;

        public IQueryService<Client> queryClients { get; set; }
        public IQueryService<User> queryUsers { get; set; }
        public IQueryService<Alert> queryAlerts { get; set; }

        public Alert alert;
        public Client client;
        public User user;

        public Guid alertId;
        public Guid clientId;
        public Guid userId;

        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            StubUserAndItsClient();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryAlerts = MockRepository.GenerateMock<IQueryService<Alert>>();
            queryClients = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();
        }

        private void Setup_Controller()
        {
            controller = new AlertsController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] {});
            FakeControllerContext.Initialize(controller);

            controller.QueryAlerts = queryAlerts;
        }

        private void SetUp_StubData()
        {
            alertId = Guid.NewGuid();
            alert = MockRepository.GeneratePartialMock<Alert>();
            alert.Stub(c => c.Id).Return(alertId);
            alert.Contact = "me@yahoo.com";
            alert.LowLevelStock = "R - 0";
            alert.OutpostId = Guid.NewGuid();
            alert.OutpostName = "Outpost1";
            alert.OutpostStockLevelId = Guid.NewGuid();
            alert.ProductGroupId = Guid.NewGuid();
            alert.ProductGroupName = "PG1";
            alert.LastUpdate = DateTime.UtcNow.AddDays(-2);
            alert.Client = client;
        }

        public void StubUserAndItsClient()
        {
            queryClients = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();

            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = CLIENT_NAME;

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = USER_NAME;
            user.Password = "4321";

            queryClients.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] {user}.AsQueryable());

            controller.QueryClients = queryClients;
            controller.QueryUsers = queryUsers;

        }

        public IQueryable<Alert> PageOfAlertsData(AlertsIndexModel indexModel)
        {
            var alertsPageList = new List<Alert>();

            Debug.Assert(indexModel.start != null, "indexModel.start != null");
            Debug.Assert(indexModel.limit != null, "indexModel.limit != null");
            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                alertsPageList.Add(new Alert
                    {
                        Contact = alert.Contact,
                        LowLevelStock = alert.LowLevelStock,
                        OutpostId = alert.OutpostId,
                        OutpostName = alert.OutpostName + " " + i,
                        OutpostStockLevelId = alert.OutpostStockLevelId,
                        ProductGroupId = alert.ProductGroupId,
                        ProductGroupName = alert.ProductGroupName,
                        LastUpdate = DateTime.UtcNow.AddDays(-i),
                        Client = alert.Client,
                        AlertType = (AlertType) (i%3)
                    });
            }
            return alertsPageList.AsQueryable();
        }
    }
}
