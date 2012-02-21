﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.ClientManager;
using Domain;
using AutoMapper;
using Web.Models.Shared;
using Core.Persistence;
using Core.Domain;

namespace Web.Controllers
{
    public class ClientManagerController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public ISaveOrUpdateCommand<Client> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Client> DeleteCommand { get; set; }

        [HttpGet]
        public ViewResult Overview()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(ClientManagerModel inputModel)
        {
            var client = new Client();
            CreateMapping();

            Mapper.Map(inputModel, client);

            SaveOrUpdateCommand.Execute(client);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Client {0} has been saved.", inputModel.Name)
               });
        }

        [HttpPost]
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

            var client = QueryClients.Load(inputModel.Id);
            CreateMapping();

            Mapper.Map(inputModel, client);

            SaveOrUpdateCommand.Execute(client);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Client {0} has been saved.", inputModel.Name)
               });
        }

        [HttpPost]
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
        public JsonResult GetListOfClients(ClientManagerIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var clientsDataQuery = this.QueryClients.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Client>>>()
            {
                { "Name-ASC", () => clientsDataQuery.OrderBy(c => c.Name) },
                { "Name-DESC", () => clientsDataQuery.OrderByDescending(c => c.Name) },
                
            };

            clientsDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                clientsDataQuery = clientsDataQuery.Where(it => it.Name.Contains(indexModel.searchValue));
            }

            var totalItems = clientsDataQuery.Count();

            clientsDataQuery = clientsDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var clientsModelListProjection = (from client in clientsDataQuery.ToList()
                                            select new ClientManagerOutputModel
                                            {
                                                Id = client.Id,
                                                Name = client.Name,
                                                NoOfUsers = GetNumberOfUsers(client.Id)
                                                
                                            }).ToArray();


            return Json(new ClientIndexOutputModel
            {
                Clients = clientsModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private int GetNumberOfUsers(Guid? guid)
        {
            if (guid.HasValue)
            {
                return QueryUsers.Query().Where(it => it.ClientId == guid.Value).ToList().Count();
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
