using System;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace Migrations
{

	public static class AssignCommonColumnsExtensions
	{
		public static ICreateTableWithColumnSyntax WithCommonColumns(
			this ICreateTableWithColumnSyntax syntax)
		{
			return syntax
				.WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				.WithColumn("Created").AsDate().Nullable()
				.WithColumn("Updated").AsDate().Nullable()
				.WithColumn("ByUser_FK").AsGuid();
		}
	}

	[Migration(1)]
	public class BasicAuthenticationInfrastructure : Migration
	{
		public override void Up()
		{
			const int NAME_LENGTH = 255;
			const int DESCRIPTION_LENGTH = 500;

			Create.Table("Permissions").WithCommonColumns()
				.WithColumn("Name").AsString(NAME_LENGTH).Unique();

			Create.Table("PermissionRoles")
				.WithColumn("Permission_FK").AsGuid().NotNullable()
				.WithColumn("Role_FK").AsGuid().NotNullable();

			Create.Table("Roles").WithCommonColumns()
				.WithColumn("Name").AsString(NAME_LENGTH).Unique()
				.WithColumn("Description").AsString(DESCRIPTION_LENGTH).Nullable();

			Create.Table("RoleUsers")
				.WithColumn("User_FK").AsGuid().NotNullable()
				.WithColumn("Role_FK").AsGuid().NotNullable();

			Create.Table("Users").WithCommonColumns()
				.WithColumn("UserName").AsString(NAME_LENGTH).NotNullable().Unique()
				.WithColumn("Password").AsString(NAME_LENGTH).NotNullable()
				.WithColumn("Email").AsString(NAME_LENGTH).Nullable().Unique()
				.WithColumn("ClientId").AsGuid();


			AddForeignKey("Permissions");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
  
		private void AddForeignKey( string fromTable, string fromColumnName="ByUser_FK",
			string toTable="Users", string toColumnName="Id")
		{
			Create.ForeignKey(String.Format("{0}_{1}_{2}_FK", fromColumnName, toTable, fromTable))
				  .FromTable(fromTable)
				  .ForeignColumn(fromColumnName)
				  .ToTable(toTable)
				  .PrimaryColumn(toColumnName);
		}	
	}
}
