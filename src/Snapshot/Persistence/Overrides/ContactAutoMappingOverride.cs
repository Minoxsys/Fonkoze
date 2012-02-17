using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Core.Domain;
using Domain;

namespace Persistence.Overrides
{
    public class ContactAutoMappingOverride : IAutoMappingOverride<Contact>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Contact> mapping)
        {
            mapping.References(p => p.Outpost, "Outpost_FK");
        }
    }
}
