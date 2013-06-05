using AutoMapper;
using Core.Domain;
using Core.Persistence;
using Core.Security;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.CustomFilters;
using Web.Models.ClientManager;
using Web.Models.Shared;
using Web.Security;

namespace Web.Controllers
{
    public class ClientManagerController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public ISaveOrUpdateCommand<Client> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Client> DeleteCommand { get; set; }
        public IPermissionsService PermissionService { get; set; }

        private const String ClientAddPermission = "Client.Edit";
        private const String ClientDeletePermission = "Client.Delete";

        [HttpGet]
        [Requires(Permissions = "Client.View")]
        public ViewResult Overview()
        {
            ViewBag.HasNoRightsToAdd = PermissionService.HasPermissionAssigned(ClientAddPermission, User.Identity.Name)
                                           ? false.ToString().ToLowerInvariant()
                                           : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = PermissionService.HasPermissionAssigned(ClientDeletePermission, User.Identity.Name)
                                              ? false.ToString().ToLowerInvariant()
                                              : true.ToString().ToLowerInvariant();

            return View();
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Create(ClientManagerModel inputModel)
        {
            var client = new Client();
            CreateMapping();

            Mapper.Map(inputModel, client);

            var existingClientsWithSameName = QueryClients.Query().Where(it => it.Name == inputModel.Name);

            if (existingClientsWithSameName.Any())
            {
                return Json(
                    new JsonActionResponse
                        {
                            Status = "Error",
                            Message = String.Format("There already exist a client with name {0}. Please insert a different name!", inputModel.Name)
                        });

            }

            SaveOrUpdateCommand.Execute(client);

            return Json(
                new JsonActionResponse
                    {
                        Status = "Success",
                        Message = String.Format("Client {0} has been created.", inputModel.Name)
                    });
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Edit(ClientManagerModel inputModel)
        {

            if (inputModel.Id == Guid.Empty)
            {
                return Json(
                    new JsonActionResponse
                        {
                            Status = "Error",
                            Message = "You must supply a clientId in order to edit the client."
                        });
            }

            var existingClientsWithSameName = QueryClients.Query().Where(it => it.Name == inputModel.Name && it.Id != inputModel.Id);

            if (existingClientsWithSameName.Count() > 0)
            {
                return Json(
                    new JsonActionResponse
                        {
                            Status = "Error",
                            Message = String.Format("There already exist a client with name {0}. Please insert a different name!", inputModel.Name)
                        });

            }

            var client = QueryClients.Load(inputModel.Id);
            CreateMapping();

            Mapper.Map(inputModel, client);

            SaveOrUpdateCommand.Execute(client);

            return Json(
                new JsonActionResponse
                    {
                        Status = "Success",
                        Message = String.Format("Client {0} has been updated.", inputModel.Name)
                    });
        }

        [HttpPost]
        [ApplicationActivityFilter]
        public JsonResult Delete(Guid? clientId)
        {
            if (clientId.HasValue == false)
            {
                return Json(new JsonActionResponse
                    {
                        Status = "Error",
                        Message = "You must supply a clientId in order to remove the client."
                    });
            }

            var client = QueryClients.Load(clientId.Value);
            if (client != null)
            {
                var users = QueryUsers.Query().Where(it => it.ClientId == client.Id);
                if (users.ToList().Count != 0)
                {
                    return Json(new JsonActionResponse
                        {
                            Status = "Error",
                            Message = String.Format("Client {0} has {1} user(s) associated, and can not be removed.", client.Name, users.ToList().Count)
                        });
                }
                DeleteCommand.Execute(client);
                return Json(
                    new JsonActionResponse
                        {
                            Status = "Success",
                            Message = String.Format("Client {0} was removed.", client.Name)
                        });
            }
            return Json(
                new JsonActionResponse
                    {
                        Status = "Error",
                        Message = String.Format("There is no client asociated with that id.")
                    });
        }

        [HttpGet]
        public JsonResult GetListOfClients(IndexTableInputModel indexTableInputModel)
        {
            var pageSize = indexTableInputModel.limit.Value;
            var clientsDataQuery = QueryClients.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Client>>>()
                {
                    {"Name-ASC", () => clientsDataQuery.OrderBy(c => c.Name)},
                    {"Name-DESC", () => clientsDataQuery.OrderByDescending(c => c.Name)}

                };

            clientsDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexTableInputModel.sort, indexTableInputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexTableInputModel.searchValue))
            {
                clientsDataQuery = clientsDataQuery.Where(it => it.Name.Contains(indexTableInputModel.searchValue));
            }

            var totalItems = clientsDataQuery.Count();

            clientsDataQuery = clientsDataQuery
                .Take(pageSize)
                .Skip(indexTableInputModel.start.Value);

            var clientsModelListProjection = (from client in clientsDataQuery.ToList()
                                              select new ClientManagerOutputModel
                                                  {
                                                      Id = client.Id,
                                                      Name = client.Name,
                                                      NoOfUsers = GetNumberOfUsers(client.Id)

                                                  }).ToArray();


            return Json(new StoreOutputModel<ClientManagerOutputModel>
                {
                    Items = clientsModelListProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }

        private int GetNumberOfUsers(Guid? guid)
        {
            if (guid.HasValue)
            {
                return QueryUsers.Query().Count(it => it.ClientId == guid.Value);
            }
            return 0;
        }

        private void CreateMapping()
        {
            Mapper.CreateMap<ClientManagerModel, Client>();
            Mapper.CreateMap<Client, ClientManagerModel>();
        }
    }
}
