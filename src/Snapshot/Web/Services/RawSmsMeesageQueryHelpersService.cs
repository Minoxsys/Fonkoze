using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Persistence;
using Domain;
using Domain.Enums;
using Web.Areas.MessagesManagement.Models.Messages;

namespace Web.Services
{
    public class RawSmsMeesageQueryHelpersService : IRawSmsMeesageQueryHelpersService
    {
        private readonly IQueryService<RawSmsReceived> _queryRawSmsService;
        private readonly IQueryService<Outpost> _queryOutpostService;


        public RawSmsMeesageQueryHelpersService(IQueryService<RawSmsReceived> queryRawSmsService, IQueryService<Outpost> queryOutpostService)
        {
            _queryOutpostService = queryOutpostService;
            _queryRawSmsService = queryRawSmsService;
        }

        public MessageIndexOuputModel GetMessagesFromOutpost(MessagesIndexModel indexModel, OutpostType outpostType)
        {
            Debug.Assert(indexModel.limit != null, "indexModel.limit != null");
            var pageSize = indexModel.limit.Value;
            var rawDataQuery = _queryRawSmsService.Query().Where(it => it.OutpostType == outpostType);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<RawSmsReceived>>>
                {
                    {"Sender-ASC", () => rawDataQuery.OrderBy(c => c.Sender)},
                    {"Sender-DESC", () => rawDataQuery.OrderByDescending(c => c.Sender)},
                    {"Date-ASC", () => rawDataQuery.OrderBy(c => c.ReceivedDate)},
                    {"Date-DESC", () => rawDataQuery.OrderByDescending(c => c.ReceivedDate)},
                    {"Content-ASC", () => rawDataQuery.OrderBy(c => c.Content)},
                    {"Content-DESC", () => rawDataQuery.OrderByDescending(c => c.Content)},
                    {"ParseSucceeded-ASC", () => rawDataQuery.OrderBy(c => c.ParseSucceeded)},
                    {"ParseSucceeded-DESC", () => rawDataQuery.OrderByDescending(c => c.ParseSucceeded)},
                    {"ParseErrorMessage-ASC", () => rawDataQuery.OrderBy(c => c.ParseErrorMessage)},
                    {"ParseErrorMessage-DESC", () => rawDataQuery.OrderByDescending(c => c.ParseErrorMessage)}
                };

            rawDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                rawDataQuery = rawDataQuery.Where(it => it.Content.Contains(indexModel.searchValue));

            var totalItems = rawDataQuery.Count();

            Debug.Assert(indexModel.start != null, "indexModel.start != null");
            rawDataQuery = rawDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var messagesModelListProjection = (from message in rawDataQuery.ToList()
                                               let outpost = _queryOutpostService.Load(message.OutpostId)
                                               select new MessageModel
                                                   {
                                                       Sender = message.Sender,
                                                       Date = message.ReceivedDate.ToString("dd/MM/yyyy HH:mm"),
                                                       Content = message.Content,
                                                       ParseSucceeded = message.ParseSucceeded,
                                                       ParseErrorMessage = message.ParseErrorMessage,
                                                       OutpostName = outpost != null ? outpost.Name : null
                                                   }).ToArray();


            return new MessageIndexOuputModel
                {
                    Messages = messagesModelListProjection,
                    TotalItems = totalItems
                };
        }
    }
}