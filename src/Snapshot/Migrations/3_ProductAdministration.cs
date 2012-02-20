using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
	[Migration(3)]
	public class _3_ProductAdministration : Migration
	{
		public override void Down()
		{

			Delete.RemoveForeignKey("Products", "Outpost_FK", "Outposts");
			Delete.RemoveForeignKey("Products", "ProductGroup_FK", "ProductGroups");

			Delete.RemoveClientForeignKey("ProductGroups");
			Delete.RemoveClientForeignKey("Products");
			Delete.RemoveForeignKey("ProductGroups");
			Delete.RemoveForeignKey("Products");


			Delete.Table("ProductGroups");
			Delete.Table("Products");
		}

		public override void Up()
		{
			Create.Table("ProductGroups")
				.WithCommonColumns()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("ReferenceCode").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable()
				;
			Create.AddForeignKey("ProductGroups");

			Create.Table("Products")
				.WithCommonColumns()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable()
				.WithColumn("LowerLimit").AsInt32()
				.WithColumn("UpperLimit").AsInt32()
				.WithColumn("SMSReferenceCode").AsFixedLengthString(1)
				.WithColumn("ProductGroup_FK").AsGuid()
				;
			Create.AddForeignKey("Products");
			Create.AddForeignKey("Products", "ProductGroup_FK", "ProductGroups");
		}
	}
}
