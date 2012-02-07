using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Web.Controllers;
using Domain;
using Core.Domain;
using Core.Persistence;
using Core.Services;
using Web.Security;
using Web.Models.UserManager;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    public class ObjectMother
    {
        public UserManagerController controller;

        public IQueryService<User> queryUsers;
        public IQueryService<Role> queryRole;
        public IQueryService<Client> queryClient;

        public ISaveOrUpdateCommand<User> saveCommand;
        public IDeleteCommand<User> deleteCommand;

        private ISecurePassword securePassword = new SecurePassword();

        public User user;
        public Client client;
        public Role role;
        public Guid userId;
        public Guid clientId;
        public Guid roleId;

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryRole = MockRepository.GenerateMock<IQueryService<Role>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<User>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<User>>();
        }

        private void Setup_Controller()
        {
            controller = new UserManagerController();

            controller.QueryClients = queryClient;
            controller.QueryRoles = queryRole;
            controller.QueryUsers = queryUsers;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;

            controller.SecurePassword = securePassword;
        }

        private void SetUp_StubData()
        {
            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = "Edgard";

            roleId = Guid.NewGuid();
            role = MockRepository.GeneratePartialMock<Role>();
            role.Stub(c => c.Id).Return(roleId);
            role.Name = "PM";

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(userId);
            user.Email = "eu@yahoo.com";
            user.FirstName = "Ion";
            user.LastName = "Pop";
            user.Password = "123asd";
            user.UserName = "Ion.Pop";
            user.ClientId = client.Id;
            user.ClientName = client.Name;
            user.RoleId = role.Id;
            user.RoleName = role.Name;
        }

        public IQueryable<User> PageOfUsersData(UserManagerIndexModel indexModel)
        {
            List<User> userPageList = new List<User>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                userPageList.Add(new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = i + user.UserName,
                    Email = i + user.Email,
                    Password = user.Password,
                    RoleName = user.RoleName,
                    ClientName = user.ClientName,
                });
            }
            return userPageList.AsQueryable();
        }
    }
}
