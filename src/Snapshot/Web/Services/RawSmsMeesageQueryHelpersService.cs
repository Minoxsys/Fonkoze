using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Persistence;
using Domain;
using Domain.Enums;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Models.Shared;

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

        public MessageIndexOuputModel GetMessagesFromOutpost(IndexTableInputModel indexTableInputModel, OutpostType outpostType, Guid? districtId = null)
        {
            Debug.Assert(indexTableInputModel.limit != null, "indexTableInputModel.limit != null");
            var pageSize = indexTableInputModel.limit.Value;
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

            rawDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexTableInputModel.sort, indexTableInputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexTableInputModel.searchValue))
            {
                rawDataQuery = rawDataQuery.Where(it => it.Content.Contains(indexTableInputModel.searchValue));
            }
            if (districtId != null && districtId != Guid.Empty)
            {
                rawDataQuery = rawDataQuery
                    .Where(it => _queryOutpostService.Query().Where(i => i.District.Id == districtId).Where(j => j.Id == it.OutpostId).Count() != 0);
            }

            var totalItems = rawDataQuery.Count();

            Debug.Assert(indexTableInputModel.start != null, "indexTableInputModel.start != null");
            rawDataQuery = rawDataQuery
                .Take(pageSize)
                .Skip(indexTableInputModel.start.Value);

            List<MessageModel> list = new List<MessageModel>();
            foreach (RawSmsReceived message in rawDataQuery.ToList())
            {
                Outpost outpost = _queryOutpostService.Load(message.OutpostId);
                list.Add(new MessageModel
                    {
                        Sender = message.Sender,
                        Date = message.ReceivedDate.ToString("dd/MM/yyyy HH:mm"),
                        Content = message.Content,
                        ParseSucceeded = message.ParseSucceeded,
                        ParseErrorMessage = message.ParseErrorMessage,
                        OutpostName = outpost != null ? outpost.Name : null
                    });
            }
            var messagesModelListProjection = list.ToArray();


            return new MessageIndexOuputModel
                {
                    Messages = messagesModelListProjection,
                    TotalItems = totalItems
                };
        }
    }
}