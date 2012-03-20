using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Bootstrap;
using Core.Persistence;
using Domain;
using Web.Models.EmailRequest;
using System.Net.Mail;
using Web.Helpers;
using Web.Controllers;

namespace Web.Services
{
    public class EmailRequestService : IProductLevelRequestMessageSenderService
    {
        private string _from = AppSettings.SendMailFrom;
        private const string _subject = "Stock Request";
        private const bool _isBodyHtml = true;

        private ISaveOrUpdateCommand<EmailRequest> saveOrUpdateCommand;
        private IURLService urlService;
        public IEmailService emailService;
        private FormattingStrategy formattingStrategy;

        public EmailRequestService(ISaveOrUpdateCommand<EmailRequest> saveOrUpdateCommand, IURLService urlService, IEmailService emailService,
            FormattingStrategy formattingStrategy)
        {
            this.saveOrUpdateCommand = saveOrUpdateCommand;
            this.urlService = urlService;
            this.emailService = emailService;
            this.formattingStrategy = formattingStrategy;
        }



        public bool SendProductLevelRequestMessage(ProductLevelRequestMessageInput input)
        {
            if (input.Products == null || input.Contact == null || input.Contact.ContactType == null)
                return false;

            if ((input.Products.Count > 0) && input.Contact.ContactType.Equals(Contact.EMAIL_CONTACT_TYPE))
            {
                return SaveAndSendEmailRequest(input);
            }
            return false;
        }

        private bool SaveAndSendEmailRequest(ProductLevelRequestMessageInput input)
        {
            EmailRequest emailRequestEntity = new EmailRequest()
            {
                Date = DateTime.UtcNow,
                OutpostId = input.Outpost.Id,
                ProductGroupId = input.ProductGroup.Id,
                Client = input.Client
            };

            saveOrUpdateCommand.Execute(emailRequestEntity);
            Guid EmailRequestId = emailRequestEntity.Id;

            EmailRequestModel model = new EmailRequestModel()
            {
                Id = EmailRequestId.ToString(),
                Date = emailRequestEntity.Date.ToString()
            };

            MailMessage message = CreateMailMessage(model, input.ProductGroup.Name, input.Contact.ContactDetail);

            return emailService.SendMail(message);
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
            string json = ConvertHelper.ConvertToJSON(model);
            string encodedData = ConvertHelper.EncodeTo64(json);

            string url = urlService.GetEmailLinkUrl(encodedData);
            string link = "<a href='" + url + "'> link </a>";
            string body = formattingStrategy.FormatEmail(productGroupName, link);

            return body;
        }


    }
}