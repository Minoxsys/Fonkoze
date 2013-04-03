using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class SentSms : DomainEntity
    {
        public virtual string Message { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Response { get; set; }
        public virtual DateTime SentDate { get; set; }
    }
}
