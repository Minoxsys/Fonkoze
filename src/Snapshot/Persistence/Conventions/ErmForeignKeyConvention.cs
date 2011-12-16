using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persistence.Conventions
{
	public class ErmForeignKeyConvention
		: FluentNHibernate.Conventions.ForeignKeyConvention
	{
		protected override string GetKeyName( FluentNHibernate.Member property, Type type )
		{
			if (property == null)
				return type.Name + "_FK";

			return property.Name + "_FK";

		}
		
	}
}
