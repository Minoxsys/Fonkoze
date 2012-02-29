using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Create;

namespace Migrations
{
	public static class AssignCommonColumnsExtensions
	{
		public static ICreateTableWithColumnSyntax WithCommonColumns(
			this ICreateTableWithColumnSyntax syntax)
		{
			return syntax
						 .WithColumn("Id")
						 .AsGuid()
						 .NotNullable()
						 .PrimaryKey()
						 .WithColumn("Created")
						 .AsDateTime()
						 .Nullable()
						 .WithColumn("Updated")
						 .AsDateTime()
						 .Nullable()
						 .WithColumn("ByUser_FK")
						 .AsGuid()
                         .Nullable();
		}
		public static ICreateTableWithColumnSyntax WithClientColumn(
			this ICreateTableWithColumnSyntax syntax)
		{
			return syntax
						 .WithColumn("Client_FK")
						 .AsGuid();
		}

		public static void AddForeignKey( this ICreateExpressionRoot Create, string fromTable, string fromColumnName = "ByUser_FK",
			string toTable = "Users", string toColumnName = "Id")
		{
			Create.ForeignKey(ConstraintUtility.ForeignKeyName(fromTable, fromColumnName, toTable, toColumnName))
				  .FromTable(fromTable)
				  .ForeignColumn(fromColumnName)
				  .ToTable(toTable)
				  .PrimaryColumn(toColumnName);
		}

		public static void AddClientForeignKey(this ICreateExpressionRoot Create, string fromTable, string fromColumnName = "Client_FK",
			string toTable = "Clients", string toColumnName = "Id")
		{
			Create.ForeignKey(ConstraintUtility.ForeignKeyName(fromTable, fromColumnName, toTable, toColumnName))
				  .FromTable(fromTable)
				  .ForeignColumn(fromColumnName)
				  .ToTable(toTable)
				  .PrimaryColumn(toColumnName);
		}
	}
}