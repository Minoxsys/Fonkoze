using Core.Domain;
using Domain.Enums;
using System;

namespace Domain
{
    public class RawSmsReceived : DomainEntity
    {
        public virtual string Sender { get; set; }
        public virtual string Content { get; set; }
        public virtual string Credits { get; set; }
        public virtual Guid OutpostId { get; set; }
        public virtual bool ParseSucceeded { get; set; }
        public virtual string ParseErrorMessage { get; set; }
        public virtual DateTime ReceivedDate { get; set; }
        public virtual OutpostType? OutpostType { get; set; }
    }
}
