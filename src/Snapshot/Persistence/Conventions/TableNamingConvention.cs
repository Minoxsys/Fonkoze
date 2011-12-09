using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;

namespace Persistence.Conventions
{
	public class TableNamingConvention
		:IClassConvention
	{
		public void Apply( FluentNHibernate.Conventions.Instances.IClassInstance instance )
		{
			// TODO: consider using a valid inflector here 
			// for correct english names
            if (instance.EntityType.Name == "Country")
            {
                instance.Table("Countries");
                return;
            }
            instance.Table(instance.EntityType.Name + "s");
        }
	}
}
