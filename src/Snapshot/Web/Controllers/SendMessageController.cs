using Core.Persistence;
using Domain;
using System;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Web.Bootstrap;
using Web.Models.Shared;
using Web.Security;
using Web.Services;

namespace Web.Controllers
{
    public class SendMessageController : Controller
    {
        public ISendSmsService SMSGatewayService { get; set; }
        public ISendEmailService EmailService { get; set; }
        public IHttpService HttpService { get; set; }
        public ISaveOrUpdateCommand<SentSms> SaveOrUpdateCommand { get; set; }

        [Requires(Permissions = "Client.View")]
        public ActionResult Overview()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(string phoneNumber, string message, string gateway)
        {
            if (gateway == "remote")
            {
                string postData = GeneratePostData(message, phoneNumber);

                try
                {
                    string responseString = HttpService.Post(postData);
                    SaveMessage("+" + phoneNumber, message, responseString);

                    return Json(
                        new JsonActionResponse
                            {
                                Status = "Success",
                                Message = responseString
                            });
                }
                catch (Exception ext)
                {
                    SaveMessage("+" + phoneNumber, message, ext.Message);
                    return Json(
                        new JsonActionResponse
                            {
                                Status = "Error",
                                Message = ext.Message
                            });
                }
            }
            return Json(
                new JsonActionResponse
                    {
                        Status = "Error",
                        Message = "Message not sent!"
                    });
        }

        private void SaveMessage(string sender, string message, string responseString)
        {
            SentSms sentSms = new SentSms {PhoneNumber = sender, Message = message, Response = responseString, SentDate = DateTime.UtcNow};
            SaveOrUpdateCommand.Execute(sentSms);
        }

        private string GeneratePostData(string message, string phoneNumber)
        {
            String postMessage = HttpUtility.UrlEncode(message);
            String strPost = "?phonenumber=%2B" + phoneNumber + "&user=" + AppSettings.SmsGatewayUserName + "&password=" + AppSettings.SmsGatewayPassword +
                             "&text=" + postMessage;
            return strPost;
        }

        [HttpPost]
        public JsonResult SendEmail(string message, string subject, string to, string cc)
        {
            var mail = new MailMessage();

            mail.To.Add(new MailAddress(to));
            mail.CC.Add(new MailAddress(cc));
            mail.From = new MailAddress(AppSettings.SendMailFrom);
            mail.Subject = subject;
            mail.IsBodyHtml = false;
            mail.Body = message;

            var ok = EmailService.SendMail(mail);

            return Json(
                new JsonActionResponse
                    {
                        Status = " ",
                        Message = ok
                    });
        }
    }
}
