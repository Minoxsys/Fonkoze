using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.MessagesManagement.Models.SentMessages
{
    public class SentMessageModel
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string SentDate { get; set; }
        public string Response { get; set; }
    }
}