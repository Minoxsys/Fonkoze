using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.RoleManager;
using Core.Domain;
using Persistence.Commands;
using Persistence.Queries;
using Persistence;
using Core.Persistence;
using Web.Security;
using Web.Models.Shared;
using NHibernate.Linq;
using System.Text;
using Persistence.Queries.Roles;
using Core.Security;

namespace Web.Controllers
{
	public class RoleManagerController :Controller	{
		
		public ISaveOrUpdateCommand<Role> SaveOrUpdate
		{
			get;
			set;
		}

        public IQueryService<Role> QueryServiceRole
        {
            get;
            set;
        }

        public IQueryService<User> QueryServiceUsers
        {
            get;
            set;
        }

        public IQueryService<Permission> QueryServicePermission
        {
            get;
            set;
        }

        public IDeleteCommand<Role> DeleteCommand
        {
            get;
            set;
        }

        public IQueryRole QueryRole
        {
            get;
            set;
        }

        public IPermissionsService PermissionService { get; set; }

        private const String ROLE_ADD_PERMISSION = "Role.Edit";
        private const String ROLE_DELETE_PERMISSION = "Role.Delete";

        [HttpGet]
        [Requires(Permissions = "Role.View")]
        public ViewResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(ROLE_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(ROLE_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           

            return View();
        }

        [HttpGet]
        public JsonResult GetListOfRoles(IndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var rolesDataQuery = QueryServiceRole.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Role>>>()
            {
                { "Name-ASC", () => rolesDataQuery.OrderBy(r => r.Name) },
                { "Name-DESC", () => rolesDataQuery.OrderByDescending(r => r.Name) }
            };

            rolesDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                rolesDataQuery = rolesDataQuery.Where(it => it.Name.Contains(indexModel.searchValue));
            }

            var totalItems = rolesDataQuery.Count();

            rolesDataQuery = rolesDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var roleListOfReferenceModelsProjection = (from role in rolesDataQuery.ToList()
                                                       select new RoleReferenceModel
                                                       {
                                                           Id = role.Id,
                                                           Name = role.Name,
                                                           Description = role.Description,
                                                           NumberOfUsers = GetNumberOfUsers(role.Id)
                                                       }).ToArray();

            return Json(new RoleManagerListForJsonOutput
            {
                Roles = roleListOfReferenceModelsProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private int GetNumberOfUsers(Guid roleId)
        {
            return QueryServiceUsers.Query().Where(it => it.RoleId == roleId).Count();
        }

        [HttpPost]
        [Requires(Permissions = "Role.Edit")]
        public JsonResult Create(RoleManagerInputModel inputModel)
        {
            Role role = new Role();
            var existsRoleName = QueryServiceRole.Query().Where(it => it.Name == inputModel.Name).Any();
            if (existsRoleName)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = string.Format("Role with the name {0} already exists!", inputModel.Name) }, JsonRequestBehavior.AllowGet);
            }

            UpdateRoleWithRoleManagerInputModel(role, inputModel);   

            return Json(new JsonActionResponse() { Status = "Success", Message = string.Format("Role {0} has been saved.", role.Name) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(RoleManagerInputModel inputModel)
        {
            if (inputModel.Id == Guid.Empty)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply a roleId in order to edit the role." });
            }

            Role role = QueryServiceRole.Load(inputModel.Id);

            if (role == null)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply the roleId of a role that exists in the DB in order to edit it." });
            }

            var existsRoleName = QueryServiceRole.Query().Where(it => it.Name == inputModel.Name && it.Id != role.Id).Any();
            if (existsRoleName)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = string.Format("Role with name {0} already exists!", inputModel.Name) }, JsonRequestBehavior.AllowGet);
            }

            UpdateRoleWithRoleManagerInputModel(role, inputModel);

            return Json(new JsonActionResponse() { Status = "Success", Message = string.Format("Role {0} has been saved.", role.Name) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(Guid? roleId)
        {
            if (!roleId.HasValue)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply a roleId in order to remove the role." });
            }

            Role role = QueryServiceRole.Load(roleId.Value);

            if (role == null)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = "You must supply the roleId of a role that exists in the DB in order to remove it." });
            }

            var numberOfUsers = GetNumberOfUsers(roleId.Value);
            if (numberOfUsers > 0)
            {
                return Json(new JsonActionResponse() { Status = "Error", Message = string.Format("This role is associated with {0} users. It cannot be deleted!", numberOfUsers) });
            }

            DeleteCommand.Execute(role);

            return Json(new JsonActionResponse() { Status = "Success", Message = "Role " + role.Name+ " was removed." }); ;
        }

        [HttpGet]
        public JsonResult GetPermissionsForRole(Guid? roleId)
        {
            Role role = QueryRole.GetPermissions().Where(r => r.Id == roleId.Value).FirstOrDefault();

            string[] permissions = new string[role.Functions.Count];

            for (int i = 0; i < role.Functions.Count; i++)
            {
                permissions[i] = role.Functions[i].Name;
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        private void UpdateRoleWithRoleManagerInputModel(Role role, RoleManagerInputModel inputModel)
        {
            role.Name = inputModel.Name;
            role.Description = inputModel.Description;

            var inputPermissionNames = inputModel.PermissionNames != null ? inputModel.PermissionNames.Split(';').ToList() : new List<string>();
            var permissions = QueryServicePermission.Query().Where(p => inputPermissionNames.Contains(p.Name)).ToList();
            permissions.Add(QueryServicePermission.Query().Where(p=>p.Name == "Home.Index").FirstOrDefault());
            role = UpdatePermissionsForRole(role, permissions);

            SaveOrUpdate.Execute(role);
        }

        private Role UpdatePermissionsForRole(Role role, List<Permission> permissions)
        {
            List<Permission> permissionsToRemove = new List<Permission>();

            foreach (Permission p in role.Functions)
            {
                if (!permissions.Contains(p))
                {
                    permissionsToRemove.Add(p);
                }
            }

            foreach (Permission p in permissionsToRemove)
            {
                role.RemoveFunction(p);
            }

            foreach (Permission p in permissions)
            {
                if (!role.Functions.Contains(p))
                {
                    role.AddFunction(p);
                }
            }

            return role;
        }
	}
}
