using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Persistence.Queries.Contacts;
using System.Net.Mail;
using Web.Security;
using System.Text;
using Web.Models.EmailRequest;
using System.Web.Routing;
using Web.Services;
using Web.Bootstrap;

namespace Web.Controllers
{
    public class EmailRequestController : Controller
    {
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<ProductGroup> QueryProductGroup { get; set; }
        public IQueryService<Contact> QueryContact { get; set; }
        public IQueryService<EmailRequest> QueryEmailRequest { get; set; }
        public ISaveOrUpdateCommand<EmailRequest> SaveOrUpdateCommand { get; set; }
        public IEmailService EmailService { get; set; }
        public IURLService UrlService { get; set; }

        private const int numberOfDaysAfterTheEmailExpires = 3;

        private string _from = AppSettings.SendMailFrom;
        private const string _subject = "Stock Request";
        private const bool _isBodyHtml = true;

        public ActionResult Create()
        {
            EmailCreateModel model = new EmailCreateModel();
            model.Outposts = GetListOfAllOutposts();
            model.ProductGroups = GetListOfAllProductGroups();

            return View(model);
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

        [HttpPost]
        public ActionResult Create(EmailCreateModel model)
        {
            model.Outposts = GetListOfAllOutposts();
            model.ProductGroups = GetListOfAllProductGroups();

            if (!ModelState.IsValid)
                return View("Create", model);

            string emailAdress = GetEmailAdressIfMainContactMethodIsAnEmail(model.Outpost.Id);
            if (!string.IsNullOrEmpty(emailAdress))
                if (SaveAndSendEmailRequest(model.Outpost.Id, model.ProductGroup.Id, emailAdress))
                    return RedirectToAction("Overview", "EmailRequest");

            return RedirectToAction("Index", "Home");
        }

        private string GetEmailAdressIfMainContactMethodIsAnEmail(Guid outpostId)
        {
            var queryResultContacts = QueryContact.Query(new ContactByOutpostIdAndMainMethod(outpostId));
            Contact contact = queryResultContacts.FirstOrDefault();

            if (contact != null)
                if (contact.ContactType == Contact.EMAIL_CONTACT_TYPE)
                    return contact.ContactDetail;

            return null;
        }

        private bool SaveAndSendEmailRequest(Guid outpostId, Guid productGroupId, string emailAdress)
        {
            EmailRequest emailRequestEntity = new EmailRequest()
            {
                Date = DateTime.UtcNow,
                OutpostId = outpostId,
                ProductGroupId = productGroupId
            };

            SaveOrUpdateCommand.Execute(emailRequestEntity);
            Guid EmailRequestId = emailRequestEntity.Id;

            EmailRequestModel model = new EmailRequestModel()
            {
                Id = EmailRequestId.ToString(),
                Date = emailRequestEntity.Date.ToString()
            };

            ProductGroup productGroup = QueryProductGroup.Load(productGroupId);
            MailMessage message = CreateMailMessage(model, productGroup.Name, emailAdress);

            return EmailService.SendMail(message);
        }

        private MailMessage CreateMailMessage(EmailRequestModel model, string productGroupName, string emailAddress)
        {
            MailMessage mail = new MailMessage();

            if (!string.IsNullOrEmpty(emailAddress))
                mail.To.Add(new MailAddress(emailAddress));

            if (!string.IsNullOrEmpty(_from))
                mail.From = new MailAddress(_from);
            mail.Subject = _subject;
            mail.IsBodyHtml = _isBodyHtml;
            mail.Body = GenerateMessageBody(model, productGroupName);

            return mail;
        }

        private string GenerateMessageBody(EmailRequestModel model, string productGroupName)
        {
            string json = ConvertToJSON(model);
            string encodedData = EncodeTo64(json);

            string url = UrlService.GetEmailLinkUrl(encodedData);
            string link = "<a href='" + url + "'> link </a>";
            string body = "Please update the stock information for product group <b>" + productGroupName + "</b> at this " + link;

            return body;
        }

        private string ConvertToJSON(EmailRequestModel model)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(model);
        }

        public string EncodeTo64(string toEncode)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(toEncode)); ;
        }

        [Requires(Permissions = "Home.Index")]
        public ActionResult Response(string id)
        {
            string decodedData = DecodeFrom64(id);

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            EmailRequestModel model = oSerializer.Deserialize<EmailRequestModel>(decodedData);

            DateTime modelDate = DateTime.Parse(model.Date);

            if (NumberOfDaysBetween(DateTime.UtcNow, modelDate) < numberOfDaysAfterTheEmailExpires)
            {
                EmailRequest email = QueryEmailRequest.Load(new Guid(model.Id));
                if (email != null)
                {
                    Outpost outpost = QueryOutpost.Load(email.OutpostId);
                    return RedirectToRoute(Web.Areas.StockAdministration.StockAdministrationAreaRegistration.DEFAULT_ROUTE, new RouteValueDictionary(new { controller = "OutpostStockLevel", action = "Overview"}));
                }
            }

            return RedirectToAction("Index", "Home");
        }

        private int NumberOfDaysBetween(DateTime maxDate, DateTime minDate)
        {
            TimeSpan span = maxDate - minDate;
            return (int)span.TotalDays;
        }

        private string DecodeFrom64(string encodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedData));
        }

        public object Overview()
        {
            var model = new EmailCreateModel();

            return View(model);
        }

    }
}
