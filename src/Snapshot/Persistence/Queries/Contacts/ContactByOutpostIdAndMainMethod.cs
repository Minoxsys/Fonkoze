using System;
using Domain;
using Core.Persistence;
using System.Linq.Expressions;

namespace Persistence.Queries.Contacts
{
    public class ContactByOutpostIdAndMainMethod : IDomainQuery<Contact>
    {
        public ContactByOutpostIdAndMainMethod(Guid id)
        {
            Expression = add => add.Outpost.Id == id && add.IsMainContact == true;
        }

        public Expression<Func<Contact, bool>> Expression { get; private set; }
    }
}
