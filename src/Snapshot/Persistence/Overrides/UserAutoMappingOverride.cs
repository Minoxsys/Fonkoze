using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Core.Domain;

namespace Persistence.Overrides
{
    public class UserAutoMappingOverride : IAutoMappingOverride<User>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<User> mapping)
        {
            mapping.Map(p => p.UserName).Unique();
        }
    }
}
