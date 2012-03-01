using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
	[Migration(4)]
	public class OutpostStockLevels : Migration
	{
		public override void Down()
		{
			Delete.RemoveClientForeignKey("OutpostStockLevels");
			Delete.RemoveForeignKey("OutpostStockLevels");

			Delete.RemoveForeignKey("OutpostHistoricalStockLevels");

			Delete.Table("OutpostStockLevels");
			Delete.Table("OutpostHistoricalStockLevels");
		}

		public override void Up()
		{

			Create.Table("OutpostStockLevels")
				.WithCommonColumns()
				.WithClientColumn()
				.WithColumn("Outpost_FK").AsGuid()
				.WithColumn("Product_FK").AsGuid()
				.WithColumn("ProductGroup_FK").AsGuid()
				.WithColumn("StockLevel").AsInt32()
				.WithColumn("PrevStockLevel").AsInt32()
                .WithColumn("UpdateDate").AsDateTime().Nullable()
				.WithColumn("UpdateMethod").AsString(ConstraintUtility.NAME_LENGTH);


			Create.AddClientForeignKey("OutpostStockLevels");
			Create.AddForeignKey("OutpostStockLevels");

            Create.Table("OutpostHistoricalStockLevels")
                .WithCommonColumns()
                .WithColumn("ClientId").AsGuid()
                .WithColumn("OutpostId").AsGuid()
                .WithColumn("ProductId").AsGuid()
                .WithColumn("ProductGroupId").AsGuid()
                .WithColumn("OutpostName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("ProductGroupName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("ProductName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("ProdSMSRef").AsString(20)
                .WithColumn("StockLevel").AsInt32()
                .WithColumn("PrevStockLevel").AsInt32()
                .WithColumn("UpdateMethod").AsString(ConstraintUtility.NAME_LENGTH).WithDefaultValue("SMS")
                .WithColumn("UpdateDate").AsDateTime();

			Create.AddForeignKey("OutpostHistoricalStockLevels");

		}
	}
}
