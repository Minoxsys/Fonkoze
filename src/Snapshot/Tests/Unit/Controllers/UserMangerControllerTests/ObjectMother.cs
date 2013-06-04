using System.Diagnostics;
using Core.Domain;
using Core.Persistence;
using Core.Security;
using Core.Services;
using Domain;
using MvcContrib.TestHelper.Fakes;
using Persistence.Security;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Controllers;
using Web.Models.Shared;
using Web.Models.UserManager;
using Web.Security;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    public class ObjectMother
    {
        public UserManagerController Controller;

        public IQueryService<User> QueryUsers;
        public IQueryService<Role> QueryRole;
        public IQueryService<Client> QueryClient;

        public IQueryService<Permission> QueryPermission;

        public ISaveOrUpdateCommand<User> SaveCommand;
        public IDeleteCommand<User> DeleteCommand;

        public ISecurePassword SecurePassword;
        public IPermissionsService PermissionService;

        public User User;
        public Client Client;
        public Permission Permission;
        public Guid PermissionId;
        public Role Role;
        public Guid UserId;
        public Guid ClientId;
        public Guid RoleId;

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
            StubPermission();
        }

        private void MockServices()
        {
            QueryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            QueryRole = MockRepository.GenerateMock<IQueryService<Role>>();
            QueryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            SaveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<User>>();
            DeleteCommand = MockRepository.GenerateMock<IDeleteCommand<User>>();
            QueryPermission = MockRepository.GenerateMock<IQueryService<Permission>>();
            SecurePassword = MockRepository.GenerateMock<ISecurePassword>();
        }

        private void Setup_Controller()
        {
            Controller = new UserManagerController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("admin"), new string[] {});
            FakeControllerContext.Initialize(Controller);
            PermissionService = new FunctionRightsService(QueryPermission, QueryUsers);
            Controller.QueryClients = QueryClient;
            Controller.QueryRoles = QueryRole;
            Controller.QueryUsers = QueryUsers;
            Controller.SaveOrUpdateCommand = SaveCommand;
            Controller.DeleteCommand = DeleteCommand;
            Controller.PermissionService = PermissionService;
            Controller.SecurePassword = SecurePassword;
        }

        private void StubPermission()
        {
            PermissionId = Guid.NewGuid();
            Permission = MockRepository.GeneratePartialMock<Permission>();
            Permission.Stub(c => c.Id).Return(PermissionId);
            Permission.Roles = new List<Role>();
            Permission.Roles.Add(Role);

        }

        private void SetUp_StubData()
        {
            ClientId = Guid.NewGuid();
            Client = MockRepository.GeneratePartialMock<Client>();
            Client.Stub(c => c.Id).Return(ClientId);
            Client.Name = "Edgard";

            RoleId = Guid.NewGuid();
            Role = MockRepository.GeneratePartialMock<Role>();
            Role.Stub(c => c.Id).Return(RoleId);
            Role.Name = "PM";



            UserId = Guid.NewGuid();
            User = MockRepository.GeneratePartialMock<User>();
            User.Stub(c => c.Id).Return(UserId);
            User.Email = "eu@yahoo.com";
            User.FirstName = "Ion";
            User.LastName = "Pop";
            User.Password = "123asd";
            User.UserName = "admin";
            User.ClientId = Client.Id;
            User.RoleId = Role.Id;
        }

        public IQueryable<User> PageOfUsersData(IndexTableInputModel indexTableInputModel)
        {
            var userPageList = new List<User>();

            Debug.Assert(indexTableInputModel.start != null, "IndexTableInputModel.start != null");
            Debug.Assert(indexTableInputModel.limit != null, "IndexTableInputModel.limit != null");
            for (int i = indexTableInputModel.start.Value; i < indexTableInputModel.limit.Value; i++)
            {
                userPageList.Add(new User
                    {
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        UserName = i + User.UserName,
                        Email = i + User.Email,
                        Password = User.Password
                    });
            }
            return userPageList.AsQueryable();
        }

        public UserManagerInputModel CreatePopulatedUser()
        {
            return new UserManagerInputModel
                {
                    Id = User.Id,
                    ClientId = Client.Id,
                    UserName = User.UserName,
                    Email = User.Email,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Password = User.Password,
                    RoleId = User.RoleId
                };
        }
    }
}
