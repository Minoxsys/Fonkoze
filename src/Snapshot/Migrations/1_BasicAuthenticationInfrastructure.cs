﻿using System;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace Migrations
{

	[Migration(1)]
	public class BasicAuthenticationInfrastructure : Migration
	{
		public override void Up()
		{
			

			Create.Table("Permissions").WithCommonColumns()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH).Unique();

			Create.Table("PermissionRoles")
				.WithColumn("Permission_FK").AsGuid().NotNullable()
				.WithColumn("Role_FK").AsGuid().NotNullable();

			Create.Table("Roles").WithCommonColumns()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH).Unique()
				.WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();

			Create.Table("RoleUsers")
				.WithColumn("User_FK").AsGuid().NotNullable()
				.WithColumn("Role_FK").AsGuid().NotNullable();

			Create.Table("Users").WithCommonColumns()
				.WithColumn("UserName").AsString(ConstraintUtility.NAME_LENGTH).NotNullable().Unique()
				.WithColumn("Password").AsString(ConstraintUtility.NAME_LENGTH).NotNullable()

				.WithColumn("FirstName").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
				.WithColumn("LastName").AsString(ConstraintUtility.NAME_LENGTH).Nullable()

				.WithColumn("RoleName").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
				.WithColumn("ClientName").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
				.WithColumn("Email").AsString(ConstraintUtility.NAME_LENGTH).Nullable().Unique()

				.WithColumn("RoleId").AsGuid().Nullable()
				.WithColumn("ClientId").AsGuid();


			Create.AddForeignKey("Permissions");
			Create.AddForeignKey("Roles");
		}

		public override void Down()
		{
			Delete.RemoveForeignKey("Roles");
			Delete.RemoveForeignKey("Permissions");

			Delete.Table("PermissionRoles");
			Delete.Table("Permissions");
			Delete.Table("RoleUsers");
			Delete.Table("Roles");
			Delete.Table("Users");
		}
  
		

		
	}
}
