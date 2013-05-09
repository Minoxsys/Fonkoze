using Core.Domain;

namespace Domain
{
    public class ApplicationActivity : DomainEntity
    {
        public virtual string Message { get; set; }
    }
}
