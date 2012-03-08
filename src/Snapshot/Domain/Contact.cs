using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Domain;

namespace Domain
{
    public class Contact : DomainEntity
    {
        public const string EMAIL_CONTACT_TYPE = "Email";
        public const string MOBILE_NUMBER_CONTACT_TYPE = "Phone";

        /// <summary>
        /// Possible values: Contact.EMAIL_CONTACT_TYPE, Contact.MOBILE_NUMBER_CONTACT_TYPE
        /// </summary>
        public virtual string ContactType { get; set; }
        public virtual string ContactDetail { get; set; }
        public virtual bool IsMainContact { get; set; }
        public virtual Outpost Outpost { get; set; }
        public virtual Client Client { get; set; }
    }
}
