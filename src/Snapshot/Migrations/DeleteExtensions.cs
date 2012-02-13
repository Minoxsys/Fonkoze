using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator.Builders.Delete;

namespace Migrations
{
	public static class DeleteExtensions
	{
		public static void RemoveForeignKey(this IDeleteExpressionRoot Delete, string fromTable, string fromColumnName = "ByUser_FK",
			string toTable = "Users", string toColumnName = "Id")
		{
			Delete.ForeignKey(ConstraintUtility.ForeignKeyName(fromTable, fromColumnName, toTable, toColumnName))
				  .OnTable(fromTable);
		}

		public static void RemoveClientForeignKey(this IDeleteExpressionRoot Delete, string fromTable, string fromColumnName = "Client_FK",
			string toTable = "Clients", string toColumnName = "Id")
		{
			Delete.ForeignKey(ConstraintUtility.ForeignKeyName(fromTable, fromColumnName, toTable, toColumnName))
				  .OnTable(fromTable);
		}
	}
}
