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
using Core.Security;
using Persistence.Security;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    public class ObjectMother
    {
        public UserManagerController controller;

        public IQueryService<User> queryUsers;
        public IQueryService<Role> queryRole;
        public IQueryService<Client> queryClient;

        public IQueryService<Permission> queryPermission;

        public ISaveOrUpdateCommand<User> saveCommand;
        public IDeleteCommand<User> deleteCommand;

        private ISecurePassword securePassword = new SecurePassword();
        public IPermissionsService PermissionService;

        public User user;
        public Client client;
        public Permission permission;
        public Guid permissionId;
        public Role role;
        public Guid userId;
        public Guid clientId;
        public Guid roleId;

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
            StubPermission();
        }

        private void MockServices()
        {
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryRole = MockRepository.GenerateMock<IQueryService<Role>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<User>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<User>>();
            queryPermission = MockRepository.GenerateMock<IQueryService<Permission>>();
        }

        private void Setup_Controller()
        {
            controller = new UserManagerController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("admin"), new string[] { });
            FakeControllerContext.Initialize(controller);
            PermissionService = new FunctionRightsService(queryPermission, queryUsers);
            controller.QueryClients = queryClient;
            controller.QueryRoles = queryRole;
            controller.QueryUsers = queryUsers;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.PermissionService = PermissionService;
            controller.SecurePassword = securePassword;
        }
        private void StubPermission()
        {
            permissionId = Guid.NewGuid();
            permission = MockRepository.GeneratePartialMock<Permission>();
            permission.Stub(c => c.Id).Return(permissionId);
            permission.Roles = new List<Role>();
            permission.Roles.Add(role);

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
            user.UserName = "admin";
            user.ClientId = client.Id;
            user.RoleId = role.Id;
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
                    Password = user.Password
                });
            }
            return userPageList.AsQueryable();
        }
    }
}
