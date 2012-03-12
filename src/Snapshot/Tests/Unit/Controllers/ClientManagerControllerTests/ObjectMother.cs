using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Persistence;
using Core.Domain;
using Domain;
using Rhino.Mocks;
using Web.Models.ClientManager;
using MvcContrib.TestHelper.Fakes;
using Core.Security;
using Persistence.Security;

namespace Tests.Unit.Controllers.ClientManagerControllerTests
{
    public class ObjectMother
    {
        public ClientManagerController controller;

        public IQueryService<User> queryUsers;
        public IQueryService<Client> queryClient;
        public IQueryService<Permission> queryPermission;

        public ISaveOrUpdateCommand<Client> saveCommand;
        public IDeleteCommand<Client> deleteCommand;
        public IPermissionsService PermissionService;

        public User user;
        public Client client;
        public Permission permission;
        public Guid userId;
        public Guid clientId;
        public Guid permissionId;

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Client>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Client>>();
            queryPermission = MockRepository.GenerateMock<IQueryService<Permission>>();
            PermissionService = new FunctionRightsService(queryPermission, queryUsers);
        }

        private void Setup_Controller()
        {
            controller = new ClientManagerController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("admin"), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryUsers = queryUsers;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.PermissionService = PermissionService;
        }

        private void SetUp_StubData()
        {
            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = "Edgard";

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(userId);
            user.Email = "eu@yahoo.com";
            user.FirstName = "Ion";
            user.LastName = "Pop";
            user.Password = "123asd";
            user.UserName = "Ion.Pop";
            user.ClientId = client.Id;

            permissionId = Guid.NewGuid();
            permission = MockRepository.GeneratePartialMock<Permission>();
            permission.Stub(it => it.Id).Return(permissionId);
        }

        public IQueryable<Client> PageOfClientsData(ClientManagerIndexModel indexModel)
        {
            List<Client> clientPageList = new List<Client>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                clientPageList.Add(new Client
                {
                    Name = i + client.Name
                });
            }
            return clientPageList.AsQueryable();
        }
    }
}
