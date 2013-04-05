using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Web.Services;

namespace Web.Controllers
{
    public class SmsReceiveEndpointController : Controller
    {
        public IQueryService<Outpost> QueryOutpost { get; set; }

        public ISmsRequestService SmsRequestService { get; set; }
        public ISmsGatewayService SmsGatewayService { get; set; }

        public ISaveOrUpdateCommand<RawSmsReceived> SaveCommandRawSmsReceived { get; set; }
        public ISaveOrUpdateCommand<Alert> SaveAlertCmd { get; set; }

        [HttpGet]
        public ActionResult ReceiveSms(string sender, string content, string inNumber, string email, string credits)
        {
            var rawSmsReceived = new RawSmsReceived
                {
                    Sender = sender,
                    Content = content,
                    Credits = credits,
                    ReceivedDate = DateTime.UtcNow,
                };

            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            rawSmsReceived = SmsGatewayService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);
            if (IsUnknownOutpost(rawSmsReceived.OutpostId)) //the sender was not recognized
            {
                //TODO: send message back to sender
                return new EmptyResult();
            }

            RawSmsReceivedParseResult parseResult = SmsGatewayService.ParseRawSmsReceived(rawSmsReceived);

            rawSmsReceived.ParseSucceeded = parseResult.ParseSucceeded;
            rawSmsReceived.ParseErrorMessage = parseResult.ParseErrorMessage;
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            if (parseResult.ParseSucceeded)
            {
                SmsRequestService.UpdateOutpostStockLevelsWithValuesReceivedBySms(parseResult.SmsReceived);
            }
            else
            {
                var outpost = QueryOutpost.Query().FirstOrDefault(o => o.Id == rawSmsReceived.OutpostId);
                Debug.Assert(outpost != null, "outpost != null");
                var alert = new Alert
                    {
                        AlertType = AlertType.Error,
                        Client = outpost.Client,
                        OutpostId = rawSmsReceived.OutpostId,
                        Contact = sender,
                        OutpostName = outpost.Name,
                        ProductGroupName = "-",
                        LowLevelStock = "-",
                        LastUpdate = null
                    };
                SaveAlertCmd.Execute(alert);
            }

            return null;
        }

        private bool IsUnknownOutpost(Guid outpostId)
        {
            return outpostId == Guid.Empty;
        }
    }
}