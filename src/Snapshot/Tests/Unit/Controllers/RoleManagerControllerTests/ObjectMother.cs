using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Persistence;
using Core.Domain;
using Rhino.Mocks;
using Web.Models.RoleManager;
using Persistence.Queries.Roles;
using Web.Models.Shared;
using Core.Security;
using Persistence.Security;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    public class ObjectMother
    {
        public const string ROLE_NAME = "Country Manager";
        public const string ROLE_NAME2 = "Region Manager";
        public const string ROLE_DESCRIPTION = "This role gives you access to manage countries.";

        public RoleManagerController controller;
        
        public IQueryService<Role> queryServiceRole;
        public IQueryService<User> queryServiceUser;
        public IQueryService<Permission> queryServicePermission;
        public ISaveOrUpdateCommand<Role> saveCommandRole;
        public IDeleteCommand<Role> deleteCommandRole;
        public IQueryService<Permission> queryPermission;

        public IQueryRole queryRole;

        public Guid roleId;
        public Role role;
        public Guid roleId2;
        public Role role2;

        public Guid permissionId;
        public Permission permission;

        public Guid userId;
        public User user;

        public IPermissionsService PermissionService;
        public IndexModel indexModel;
        public RoleManagerInputModel inputModel;
        public Permission[] permissions;

        public void Init_Controller_And_Mock_Services()
        {
            controller = new RoleManagerController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("admin"), new string[] { });
            FakeControllerContext.Initialize(controller);
          

            queryServiceRole = MockRepository.GenerateMock<IQueryService<Role>>();
            queryServicePermission = MockRepository.GenerateMock<IQueryService<Permission>>();
            saveCommandRole = MockRepository.GenerateMock<ISaveOrUpdateCommand<Role>>();
            deleteCommandRole = MockRepository.GenerateMock<IDeleteCommand<Role>>();
            queryRole = MockRepository.GenerateMock<IQueryRole>();
            queryServiceUser = MockRepository.GenerateMock<IQueryService<User>>();
            queryPermission = MockRepository.GenerateMock <IQueryService<Permission>>();

            PermissionService = new FunctionRightsService(queryPermission, queryServiceUser);
            controller.PermissionService = PermissionService;

            controller.QueryServiceRole = queryServiceRole;
            controller.SaveOrUpdate = saveCommandRole;
            controller.QueryServicePermission = queryServicePermission;
            controller.DeleteCommand = deleteCommandRole;
            controller.QueryRole = queryRole;
            controller.QueryServiceUsers = queryServiceUser;
        }
        public void StubPermission()
        {
            permissionId = Guid.NewGuid();
            permission = MockRepository.GeneratePartialMock<Permission>();
            permission.Stub(it => it.Id).Return(permissionId);
            permission.Roles = new List<Role> { role };
        }
        public void Init_Stub_Data()
        {
            permissions = new Permission[] { 
                new Permission() { Name = "Country.View" }, new Permission() { Name = "Country.Edit" }, new Permission() { Name = "Country.Delete" },
                new Permission() { Name = "Region.View" }, new Permission() { Name = "Region.Delete" }, new Permission() { Name = "Region.Edit" },
                new Permission() { Name = "District.View" }, new Permission() { Name = "District.Edit" }, new Permission() { Name = "District.Delete" },
                new Permission() { Name = "Outpost.Edit" }, new Permission() { Name = "Outpost.View" }, new Permission() { Name = "Outpost.Delete" } 
            };

            roleId = Guid.NewGuid();
            role = MockRepository.GeneratePartialMock<Role>();
            role.Stub(r => r.Id).Return(roleId);
            role.Name = ROLE_NAME;
            role.Description = ROLE_DESCRIPTION;
            role.Functions = permissions.ToList();

            roleId2 = Guid.NewGuid();
            role2 = MockRepository.GeneratePartialMock<Role>();
            role2.Stub(r => r.Id).Return(roleId2);
            role2.Name = ROLE_NAME2;
            role2.Description = ROLE_DESCRIPTION;
            role2.Functions = permissions.ToList();

            StubPermission();

            indexModel = new IndexModel()
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };

            inputModel = new RoleManagerInputModel()
            {
                Id = roleId,
                Name = ROLE_NAME,
                Description = ROLE_DESCRIPTION,
                PermissionNames = "Country.View;Region.View;District.Edit;Outpost.Edit;Outpost.Delete"
            };
        }
    }
}
