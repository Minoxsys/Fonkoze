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

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    public class ObjectMother
    {
        public const string ROLE_NAME = "Country Manager";
        public const string ROLE_DESCRIPTION = "This role gives you access to manage countries.";

        public RoleManagerController controller;
        
        public IQueryService<Role> queryServiceRole;
        public IQueryService<Permission> queryServicePermission;
        public ISaveOrUpdateCommand<Role> saveCommandRole;
        public IDeleteCommand<Role> deleteCommandRole;
        public IQueryRole queryRole;

        public Guid roleId;
        public Role role;

        public RoleManagerIndexModel indexModel;
        public RoleManagerInputModel inputModel;
        public Permission[] permissions;

        public void Init_Controller_And_Mock_Services()
        {
            controller = new RoleManagerController();
            queryServiceRole = MockRepository.GenerateMock<IQueryService<Role>>();
            queryServicePermission = MockRepository.GenerateMock<IQueryService<Permission>>();
            saveCommandRole = MockRepository.GenerateMock<ISaveOrUpdateCommand<Role>>();
            deleteCommandRole = MockRepository.GenerateMock<IDeleteCommand<Role>>();
            queryRole = MockRepository.GenerateMock<IQueryRole>();

            controller.QueryServiceRole = queryServiceRole;
            controller.SaveOrUpdate = saveCommandRole;
            controller.QueryServicePermission = queryServicePermission;
            controller.DeleteCommand = deleteCommandRole;
            controller.QueryRole = queryRole;
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
            role.Employees = new List<User>();
            role.Functions = permissions.ToList();

            indexModel = new RoleManagerIndexModel()
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
