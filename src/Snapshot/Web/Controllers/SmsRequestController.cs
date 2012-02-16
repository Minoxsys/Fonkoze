using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.SmsRequest;
using Core.Persistence;
using Domain;
using Web.Services;

namespace Web.Controllers
{
    public class SmsRequestController : Controller
    {
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }

        public ISmsRequestService SmsRequestService { get; set; }
        public ISmsGatewayService SmsGatewayService { get; set; }

        public ISaveOrUpdateCommand<RawSmsReceived> SaveCommandRawSmsReceived { get; set; }

        public ActionResult Create()
        {
            SmsRequestCreateModel model = new SmsRequestCreateModel();

            model.Outposts = GetListOfAllOutposts();
            model.ProductGroups = GetListOfAllProductGroups();

            return View(model);
        }

        public ActionResult Overview()
        {
            var model = new SmsRequestCreateModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SmsRequestCreateModel model)
        {
            if (!ModelState.IsValid)
                return View("Create", model);

            if (model.Outpost.Id.Equals(Guid.Empty) || model.ProductGroup.Id.Equals(Guid.Empty))
            {
                return RedirectToAction("Create", model);
            }

            SmsRequest smsRequest = SmsRequestService.CreateSmsRequestUsingOutpostIdAndProductGroupId(model.Outpost.Id, model.ProductGroup.Id);

            if (smsRequest.Id.Equals(Guid.Empty))
            {
                return RedirectToAction("Index", "Home");
            }
            
            string result = SmsGatewayService.SendSmsRequest(smsRequest);

            return RedirectToAction("Overview", "SmsRequest");
        }

        public ActionResult ReceiveSms(string sender, string content, string inNumber, string email, string credits)
        {
            RawSmsReceived rawSmsReceived = new RawSmsReceived();
            rawSmsReceived.Sender = sender;
            rawSmsReceived.Content = content;
            rawSmsReceived.Credits = credits;

            rawSmsReceived = SmsGatewayService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            RawSmsReceivedParseResult parseResult = SmsGatewayService.ParseRawSmsReceived(rawSmsReceived);

            rawSmsReceived.ParseSucceeded = parseResult.ParseSucceeded;
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            if (parseResult.ParseSucceeded)
            {
                SmsRequestService.UpdateOutpostStockLevelsWithValuesReceivedBySms(parseResult.SmsReceived);
            }

            return null;
        }

        private List<SelectListItem> GetListOfAllOutposts()
        {
            var outPosts = new List<SelectListItem>();
            var queryResultOutposts = QueryOutpost.Query();
            if (queryResultOutposts != null)
                queryResultOutposts.ToList().ForEach(itemOutpost => outPosts.Add(new SelectListItem { Text = itemOutpost.Name, Value = itemOutpost.Id.ToString() }));
            return outPosts;
        }

        private List<SelectListItem> GetListOfAllProductGroups()
        {
            var groupProducts = new List<SelectListItem>();
            var queryResultGroupProducts = QueryProductGroup.Query();
            if (queryResultGroupProducts != null)
                queryResultGroupProducts.ToList().ForEach(groupItem => groupProducts.Add(new SelectListItem { Text = groupItem.Name, Value = groupItem.Id.ToString() }));
            return groupProducts;
        }

    }
}
