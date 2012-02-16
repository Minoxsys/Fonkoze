using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services
{
    public class RawSmsReceivedParseResult
    {
        public virtual SmsReceived SmsReceived { get; internal set; }
        public virtual bool ParseSucceeded { get; set; }
        public virtual string ParseErrorMessage { get; internal set; }
    }
}