﻿using Core.Domain;
using Core.Persistence;
using Core.Security;
using Persistence.Queries.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.CustomFilters;
using Web.Models.RoleManager;
using Web.Models.Shared;
using Web.Security;

namespace Web.Controllers
{
    public class RoleManagerController : Controller
    {
        public ISaveOrUpdateCommand<Role> SaveOrUpdate { get; set; }
        public IQueryService<Role> QueryServiceRole { get; set; }
        public IQueryService<User> QueryServiceUsers { get; set; }
        public IQueryService<Permission> QueryServicePermission { get; set; }
        public IDeleteCommand<Role> DeleteCommand { get; set; }
        public IQueryRole QueryRole { get; set; }
        public IPermissionsService PermissionService { get; set; }

        private const String RoleAddPermission = "Role.Edit";
        private const String RoleDeletePermission = "Role.Delete";

        [HttpGet]
        [Requires(Permissions = "Role.View")]
        public ViewResult Overview()
        {
            ViewBag.HasNoRightsToAdd = PermissionService.HasPermissionAssigned(RoleAddPermission, User.Identity.Name)
                                           ? false.ToString().ToLowerInvariant()
                                           : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = PermissionService.HasPermissionAssigned(RoleDeletePermission, User.Identity.Name)
                                              ? false.ToString().ToLowerInvariant()
                                              : true.ToString().ToLowerInvariant();

            return View();
        }

        [HttpGet]
        public JsonResult GetListOfRoles(IndexTableInputModel indexTableInputModel)
        {
            var pageSize = indexTableInputModel.limit.Value;
            var rolesDataQuery = QueryServiceRole.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Role>>>
                {
                    {"Name-ASC", () => rolesDataQuery.OrderBy(r => r.Name)},
                    {"Name-DESC", () => rolesDataQuery.OrderByDescending(r => r.Name)}
                };

            rolesDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexTableInputModel.sort, indexTableInputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexTableInputModel.searchValue))
            {
                rolesDataQuery = rolesDataQuery.Where(it => it.Name.Contains(indexTableInputModel.searchValue));
            }

            var totalItems = rolesDataQuery.Count();

            rolesDataQuery = rolesDataQuery
                .Take(pageSize)
                .Skip(indexTableInputModel.start.Value);

            var roleListOfReferenceModelsProjection = (from role in rolesDataQuery.ToList()
                                                       select new RoleReferenceModel
                                                           {
                                                               Id = role.Id,
                                                               Name = role.Name,
                                                               Description = role.Description,
                                                               NumberOfUsers = GetNumberOfUsers(role.Id)
                                                           }).ToArray();

            return Json(new StoreOutputModel<RoleReferenceModel>
                {
                    Items = roleListOfReferenceModelsProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }

        private int GetNumberOfUsers(Guid roleId)
        {
            return QueryServiceUsers.Query().Count(it => it.RoleId == roleId);
        }

        [HttpPost]
        [Requires(Permissions = "Role.Edit")]
        [ApplicationActivityFilter]
        public JsonResult Create(RoleManagerInputModel inputModel)
        {
            var role = new Role();
            var existsRoleName = QueryServiceRole.Query().Any(it => it.Name == inputModel.Name);
            if (existsRoleName)
            {
                return Json(new JsonActionResponse {Status = "Error", Message = string.Format("Role with the name {0} already exists!", inputModel.Name)},
                            JsonRequestBehavior.AllowGet);
            }

            UpdateRoleWithRoleManagerInputModel(role, inputModel);

            return Json(new JsonActionResponse {Status = "Success", Message = string.Format("Role {0} has been created.", role.Name)},
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Edit(RoleManagerInputModel inputModel)
        {
            if (inputModel.Id == Guid.Empty)
            {
                return Json(new JsonActionResponse {Status = "Error", Message = "You must supply a roleId in order to edit the role."});
            }

            Role role = QueryServiceRole.Load(inputModel.Id);

            if (role == null)
            {
                return
                    Json(new JsonActionResponse
                        {
                            Status = "Error",
                            Message = "You must supply the roleId of a role that exists in the DB in order to edit it."
                        });
            }

            var existsRoleName = QueryServiceRole.Query().Any(it => it.Name == inputModel.Name && it.Id != role.Id);
            if (existsRoleName)
            {
                return Json(new JsonActionResponse {Status = "Error", Message = string.Format("Role with name {0} already exists!", inputModel.Name)},
                            JsonRequestBehavior.AllowGet);
            }

            UpdateRoleWithRoleManagerInputModel(role, inputModel);

            return Json(new JsonActionResponse {Status = "Success", Message = string.Format("Role {0} has been updated.", role.Name)},
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Delete(Guid? roleId)
        {
            if (!roleId.HasValue)
            {
                return Json(new JsonActionResponse {Status = "Error", Message = "You must supply a roleId in order to remove the role."});
            }

            Role role = QueryServiceRole.Load(roleId.Value);

            if (role == null)
            {
                return
                    Json(new JsonActionResponse
                        {
                            Status = "Error",
                            Message = "You must supply the roleId of a role that exists in the DB in order to remove it."
                        });
            }

            var numberOfUsers = GetNumberOfUsers(roleId.Value);
            if (numberOfUsers > 0)
            {
                return
                    Json(new JsonActionResponse
                        {
                            Status = "Error",
                            Message = string.Format("This role is associated with {0} users. It cannot be deleted!", numberOfUsers)
                        });
            }

            DeleteCommand.Execute(role);

            return Json(new JsonActionResponse {Status = "Success", Message = "Role " + role.Name + " was removed."});
        }

        [HttpGet]
        public JsonResult GetPermissionsForRole(Guid? roleId)
        {
            Role role = QueryRole.GetPermissions().FirstOrDefault(r => r.Id == roleId.Value);

            if (role != null)
            {
                var permissions = new string[role.Functions.Count];

                for (int i = 0; i < role.Functions.Count; i++)
                {
                    permissions[i] = role.Functions[i].Name;
                }

                return Json(permissions, JsonRequestBehavior.AllowGet);
            }
            return Json(new string[] {}, JsonRequestBehavior.AllowGet);
        }

        private void UpdateRoleWithRoleManagerInputModel(Role role, RoleManagerInputModel inputModel)
        {
            role.Name = inputModel.Name;
            role.Description = inputModel.Description;

            var inputPermissionNames = inputModel.PermissionNames != null ? inputModel.PermissionNames.Split(';').ToList() : new List<string>();
            var permissions = QueryServicePermission.Query().Where(p => inputPermissionNames.Contains(p.Name)).ToList();
            permissions.Add(QueryServicePermission.Query().FirstOrDefault(p => p.Name == "Home.Index"));
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
