using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.UserManager;
using Core.Domain;
using Core.Persistence;
using Web.Security;
using AutoMapper;
using Core.Services;
using Domain;
using Web.Models.Shared;
using Core.Security;

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

        private const String USER_ADD_PERMISSION = "User.Edit";
        private const String USER_DELETE_PERMISSION = "User.Delete";

        [HttpGet]
        [Requires(Permissions = "User.View")]
        public ViewResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(USER_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(USER_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           

            return View();
        }

        [HttpPost]
        public JsonResult Create(UserManagerInputModel inputModel)
        {
            var user = new User();
            CreateMapping();

            inputModel.Password = SecurePassword.EncryptPassword(inputModel.Password);

            Mapper.Map(inputModel, user);

            SaveOrUpdateCommand.Execute(user);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Username {0} has been saved.", inputModel.UserName)
               });
        }

        [HttpPost]
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

            if (string.IsNullOrEmpty(inputModel.Password))
                inputModel.Password = user.Password;

            Mapper.Map(inputModel, user);

            SaveOrUpdateCommand.Execute(user);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Username {0} has been saved.", inputModel.UserName)
               });
        }

        [HttpPost]
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
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Username {0} was removed.", user.UserName)
                });
        }

        [HttpGet]
        public JsonResult GetListOfUsers(UserManagerIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var usersDataQuery = this.QueryUsers.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<User>>>()
            {
                { "UserName-ASC", () => usersDataQuery.OrderBy(c => c.UserName) },
                { "UserName-DESC", () => usersDataQuery.OrderByDescending(c => c.UserName) },
                { "Email-ASC", () => usersDataQuery.OrderBy(c => c.Email) },
                { "Email-DESC", () => usersDataQuery.OrderByDescending(c => c.Email) },
                { "FirstName-ASC", () => usersDataQuery.OrderBy(c => c.FirstName) },
                { "FirstName-DESC", () => usersDataQuery.OrderByDescending(c => c.FirstName) },
                { "LastName-ASC", () => usersDataQuery.OrderBy(c => c.LastName) },
                { "LastName-DESC", () => usersDataQuery.OrderByDescending(c => c.LastName) }
            };

            usersDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                usersDataQuery = usersDataQuery.Where(it => it.UserName.Contains(indexModel.searchValue));
            }

            var totalItems = usersDataQuery.Count();

            usersDataQuery = usersDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

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
                                                ClientId = user.ClientId,
                                                RoleId = user.RoleId
                                            }).ToArray();


            return Json(new UserIndexOutputModel
            {
                Users = usersModelListProjection,
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


            return Json(new ClientsIndexOutputModel
            {
                Clients = clientModelListProjection,
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


            return Json(new RolesIndexOutputModel
            {
                Roles = roleModelListProjection,
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