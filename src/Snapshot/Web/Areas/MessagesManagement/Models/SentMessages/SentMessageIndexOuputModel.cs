using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.MessagesManagement.Models.SentMessages
{
    public class SentMessageIndexOuputModel
    {
        public SentMessageModel[] Messages { get; set; }
        public int TotalItems { get; set; }
    }
}