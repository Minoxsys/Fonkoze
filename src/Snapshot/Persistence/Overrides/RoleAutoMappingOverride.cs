using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class RoleAutoMappingOverride: IAutoMappingOverride<Role>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Role> mapping)
        {
            mapping.HasManyToMany(roles => roles.Functions).Cascade.SaveUpdate();
            mapping.Map(p => p.Name).Unique();
           
        }
    }
}
