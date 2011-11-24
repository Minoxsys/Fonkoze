using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Core.Domain;

namespace Persistence.Overrides
{
    public class PermissionAutoMappingOverride : IAutoMappingOverride<Permission>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Permission> mapping)
        {
            mapping.Map(p => p.Name).Unique();
        }
    }
}
