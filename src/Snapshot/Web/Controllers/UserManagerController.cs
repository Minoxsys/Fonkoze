using AutoMapper;
using Core.Domain;
using Core.Persistence;
using Core.Security;
using Core.Services;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.CustomFilters;
using Web.Models.Shared;
using Web.Models.UserManager;
using Web.Security;

namespace Web.Controllers
{
    public class UserManagerController : Controller
    {
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Role> QueryRoles { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public ISaveOrUpdateCommand<User> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<User> DeleteCommand { get; set; }
        public ISecurePassword SecurePassword { get; set; }
        public IPermissionsService PermissionService { get; set; }

        private const String UserAddPermission = "User.Edit";
        private const String UserDeletePermission = "User.Delete";

        [HttpGet]
        [Requires(Permissions = "User.View")]
        public ViewResult Overview()
        {
            ViewBag.HasNoRightsToAdd = PermissionService.HasPermissionAssigned(UserAddPermission, User.Identity.Name)
                                           ? false.ToString().ToLowerInvariant()
                                           : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = PermissionService.HasPermissionAssigned(UserDeletePermission, User.Identity.Name)
                                              ? false.ToString().ToLowerInvariant()
                                              : true.ToString().ToLowerInvariant();

            return View();
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Create(UserManagerInputModel inputModel)
        {
            var user = new User();
            CreateMapping();

            inputModel.Password = SecurePassword.EncryptPassword(inputModel.Password);

            Mapper.Map(inputModel, user);

            var existingUsers = QueryUsers.Query().FirstOrDefault(u => u.UserName == inputModel.UserName);
            if (existingUsers == null)
            {
                SaveOrUpdateCommand.Execute(user);

                return Json(
                    new JsonActionResponse
                        {
                            Status = "Success",
                            Message = String.Format("Username {0} has been created.", inputModel.UserName)
                        });
            }
            return Json(
                new JsonActionResponse
                    {
                        Status = "Error",
                        Message = String.Format("Username {0} already exists!", inputModel.UserName)
                    });
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Edit(UserManagerInputModel inputModel)
        {
            if (inputModel.Id == Guid.Empty)
            {
                return Json(
                    new JsonActionResponse
                        {
                            Status = "Error",
                            Message = "You must supply a userId in order to edit the user."
                        });
            }

            var user = QueryUsers.Load(inputModel.Id);

            CreateMapping();

            inputModel.Password = string.IsNullOrEmpty(inputModel.Password) ? user.Password : SecurePassword.EncryptPassword(inputModel.Password);
            Mapper.Map(inputModel, user);

            SaveOrUpdateCommand.Execute(user);

            return Json(
                new JsonActionResponse
                    {
                        Status = "Success",
                        Message = String.Format("Username {0} has been updated.", inputModel.UserName)
                    });
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Delete(Guid? userId)
        {
            if (userId.HasValue == false)
            {
                return Json(new JsonActionResponse
                    {
                        Status = "Error",
                        Message = "You must supply a userId in order to remove the user."
                    });
            }

            var user = QueryUsers.Load(userId.Value);
            if (user != null)
            {
                DeleteCommand.Execute(user);

                return Json(
                    new JsonActionResponse
                        {
                            Status = "Success",
                            Message = String.Format("Username {0} was removed.", user.UserName)
                        });
            }
            return Json(
                new JsonActionResponse
                    {
                        Status = "Error",
                        Message = String.Format("Username wasnot found")
                    });
        }

        [HttpGet]
        public JsonResult GetListOfUsers(IndexTableInputModel indexTableInputModel)
        {
            var pageSize = indexTableInputModel.limit.Value;
            var usersDataQuery = QueryUsers.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<User>>>()
                {
                    {"UserName-ASC", () => usersDataQuery.OrderBy(c => c.UserName)},
                    {"UserName-DESC", () => usersDataQuery.OrderByDescending(c => c.UserName)},
                    {"Email-ASC", () => usersDataQuery.OrderBy(c => c.Email)},
                    {"Email-DESC", () => usersDataQuery.OrderByDescending(c => c.Email)},
                    {"FirstName-ASC", () => usersDataQuery.OrderBy(c => c.FirstName)},
                    {"FirstName-DESC", () => usersDataQuery.OrderByDescending(c => c.FirstName)},
                    {"LastName-ASC", () => usersDataQuery.OrderBy(c => c.LastName)},
                    {"LastName-DESC", () => usersDataQuery.OrderByDescending(c => c.LastName)}
                };

            usersDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexTableInputModel.sort, indexTableInputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexTableInputModel.searchValue))
            {
                usersDataQuery = usersDataQuery.Where(it => it.UserName.Contains(indexTableInputModel.searchValue));
            }

            var totalItems = usersDataQuery.Count();

            usersDataQuery = usersDataQuery
                .Take(pageSize)
                .Skip(indexTableInputModel.start.Value);

            var usersModelListProjection = (from user in usersDataQuery.ToList()
                                            select new UserOutputModel
                                                {
                                                    Id = user.Id,
                                                    FirstName = user.FirstName,
                                                    LastName = user.LastName,
                                                    UserName = user.UserName,
                                                    Email = user.Email,
                                                    Password = user.Password,
                                                    ClientName = GetClientName(user.ClientId),
                                                    RoleName = GetRoleName(user.RoleId),
                                                    PhoneNumber = user.PhoneNumber,
                                                    ClientId = user.ClientId,
                                                    RoleId = user.RoleId
                                                }).ToArray();


            return Json(new StoreOutputModel<UserOutputModel>
                {
                    Items = usersModelListProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }

        private string GetRoleName(Guid guid)
        {
            var role = QueryRoles.Load(guid);
            if (role != null)
                return role.Name;
            return string.Empty;
        }

        private string GetClientName(Guid guid)
        {
            var client = QueryClients.Load(guid);
            if (client != null)
                return client.Name;
            return string.Empty;
        }

        [HttpGet]
        public JsonResult GetListOfClients()
        {
            var clients = QueryClients.Query();
            int totalItems = clients.Count();

            var clientModelListProjection = (from client in clients.ToList()
                                             select new ReferenceModel
                                                 {
                                                     Id = client.Id,
                                                     Name = client.Name
                                                 }).ToArray();


            return Json(new StoreOutputModel<ReferenceModel>
                {
                    Items = clientModelListProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListOfRoles()
        {
            var roles = QueryRoles.Query();
            int totalItems = roles.Count();

            var roleModelListProjection = (from role in roles.ToList()
                                           select new ReferenceModel
                                               {
                                                   Id = role.Id,
                                                   Name = role.Name
                                               }).ToArray();


            return Json(new StoreOutputModel<ReferenceModel>
                {
                    Items = roleModelListProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }

        private void CreateMapping()
        {
            Mapper.CreateMap<UserManagerInputModel, User>();
            Mapper.CreateMap<User, UserManagerInputModel>();
        }

    }
}