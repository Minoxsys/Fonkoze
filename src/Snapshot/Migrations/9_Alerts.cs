using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(9)]
    public class Alerts : Migration
    {
        public override void Down()
        {
            Delete.RemoveClientForeignKey("Alerts");
            Delete.RemoveForeignKey("Alerts");
            Delete.Table("Alerts");
        }

        public override void Up()
        {
            Create.Table("Alerts")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("OutpostId").AsGuid()
                .WithColumn("ProductGroupId").AsGuid()
                .WithColumn("OutpostName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("ProductGroupName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("OutpostStockLevelId").AsGuid()
                .WithColumn("Contact").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("LowLevelStock").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("LastUpdate").AsDateTime();

            Create.AddClientForeignKey("Alerts");
            Create.AddForeignKey("Alerts");
        }
    }
}
