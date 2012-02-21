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

			Delete.RemoveClientForeignKey("OutpostHistoricalStockLevels");
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
				.WithColumn("UpdateMethod").AsString(ConstraintUtility.NAME_LENGTH);


			Create.AddClientForeignKey("OutpostStockLevels");
			Create.AddForeignKey("OutpostStockLevels");

            Create.Table("OutpostHistoricalStockLevels")
                .WithCommonColumns()
                .WithClientColumn()
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
            

			Create.AddClientForeignKey("OutpostHistoricalStockLevels");
			Create.AddForeignKey("OutpostHistoricalStockLevels");


            // TODO: Deprecate in version 5
            Alter.Table("OutpostStockLevels")
                .AddColumn("OutpostId").AsGuid().Nullable()
                .AddColumn("ProdGroupId").AsGuid().Nullable()
                .AddColumn("ProductId").AsGuid().Nullable()
                .AddColumn("ProductGroupName").AsString().Nullable()
                .AddColumn("ProductName").AsString().Nullable()
                .AddColumn("ProdSMSRef").AsString().Nullable();


            this.IfDatabase("sqlserver").Execute.EmbeddedScript(@"Migrations.Scripts.sqlserver_InsertIntoTables.sql");

		}
	}
}
