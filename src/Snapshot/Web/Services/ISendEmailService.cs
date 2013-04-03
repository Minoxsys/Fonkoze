using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Web.Services
{
    public interface ISendEmailService
    {
        string SendMail(MailMessage message);
    }
}