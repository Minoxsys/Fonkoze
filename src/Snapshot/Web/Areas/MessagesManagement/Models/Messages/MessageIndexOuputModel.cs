using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.MessagesManagement.Models.Messages
{
    public class MessageIndexOuputModel
    {
        public MessageModel[] Messages { get; set; }
        public int TotalItems { get; set; }
    }
}