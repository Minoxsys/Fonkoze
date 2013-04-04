using Core.Domain;
using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Models.SmsRequest;
using Web.Services;

namespace Web.Controllers
{
    public class SmsRequestController : Controller
    {
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public ISmsRequestService SmsRequestService { get; set; }
        public ISmsGatewayService SmsGatewayService { get; set; }

        public ISaveOrUpdateCommand<RawSmsReceived> SaveCommandRawSmsReceived { get; set; }
        public ISaveOrUpdateCommand<Alert> SaveAlertCmd { get; set; }

        private Client _client;
        private User _user;

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            _user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            _client = QueryClients.Load(clientId);
        }

        public ActionResult Create()
        {
            LoadUserAndClient();
            var model = new SmsRequestCreateModel
                {
                    Outposts = GetListOfAllOutposts(),
                    ProductGroups = GetListOfAllProductGroups()
                };

            return View(model);
        }

        public ActionResult Overview()
        {
            var model = new SmsRequestCreateModel();
            return View(model);
        }

        public ActionResult ReceiveSms(string sender, string content, string inNumber, string email, string credits)
        {
            var rawSmsReceived = new RawSmsReceived
                {
                    Sender = sender,
                    Content = content,
                    Credits = credits
                };

            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            rawSmsReceived = SmsGatewayService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            RawSmsReceivedParseResult parseResult = SmsGatewayService.ParseRawSmsReceived(rawSmsReceived);

            rawSmsReceived.ParseSucceeded = parseResult.ParseSucceeded;
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            if (parseResult.ParseSucceeded)
            {
                SmsRequestService.UpdateOutpostStockLevelsWithValuesReceivedBySms(parseResult.SmsReceived);
            }
            else
            {
                SaveAlertCmd.Execute(new Alert
                    {
                        AlertType = AlertType.Error,
                        Client = _client,
                        OutpostId = rawSmsReceived.OutpostId,
                        Contact = sender,
                        OutpostName = QueryOutpost.Query().First(o => o.Id == rawSmsReceived.OutpostId).Name,
                        ProductGroupName = "-",
                        LowLevelStock = "-",
                        LastUpdate = null
                    });
            }

            return null;
        }

        private List<SelectListItem> GetListOfAllOutposts()
        {
            var outPosts = new List<SelectListItem>();
            var queryResultOutposts = QueryOutpost.Query().Where(o => o.Client == _client);
            queryResultOutposts.ToList()
                               .ForEach(itemOutpost => outPosts.Add(new SelectListItem
                                   {
                                       Text = itemOutpost.Name,
                                       Value = itemOutpost.Id.ToString()
                                   }));
            return outPosts;
        }

        private List<SelectListItem> GetListOfAllProductGroups()
        {
            var groupProducts = new List<SelectListItem>();
            var queryResultGroupProducts = QueryProductGroup.Query();
            if (queryResultGroupProducts != null)
                queryResultGroupProducts.ToList()
                                        .ForEach(groupItem => groupProducts.Add(new SelectListItem
                                            {
                                                Text = groupItem.Name,
                                                Value = groupItem.Id.ToString()
                                            }));
            return groupProducts;
        }
    }
}
