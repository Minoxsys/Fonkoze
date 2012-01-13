﻿using System;
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
            mapping.HasManyToMany(roles => roles.Functions).Cascade.All();
            mapping.HasManyToMany(roles => roles.Employees).Cascade.All();

            mapping.Map(p => p.Name).Unique();
           
        }
    }
}