using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations
{
	public class ConstraintUtility
	{
		public static string ForeignKeyName(string fromTable, string fromColumnName = "ByUser_FK",
			string toTable = "Users", string toColumnName = "Id")
		{
			return String.Format("{0}_{1}_{2}_FK", fromColumnName, toTable, fromTable);
		}

		



		internal const int NAME_LENGTH = 255;
		internal const int DESCRIPTION_LENGTH = 500;
		internal const int COORDINATES_LENGTH = 50;
	}
}
