using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(8)]
    public class SmsRequest : Migration
    {
        public override void Down()
        {
            Delete.RemoveForeignKey("RawSmsReceiveds", "OutpostId", "Outposts");
            Delete.RemoveForeignKey("RawSmsReceiveds");

            Delete.Table("RawSmsReceiveds");

            Delete.RemoveForeignKey("SmsRequests", "ProductGroupId", "ProductGroups");
            Delete.RemoveForeignKey("SmsRequests", "OutpostId", "Outposts");
            Delete.RemoveForeignKey("SmsRequests");
            Delete.RemoveClientForeignKey("SmsRequests");

            Delete.Table("SmsRequests");
        }

        public override void Up()
        {
            Create.Table("SmsRequests")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Message").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Number").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("ProductGroupReferenceCode").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("OutpostId").AsGuid()
                .WithColumn("ProductGroupId").AsGuid();

            Create.AddClientForeignKey("SmsRequests");
            Create.AddForeignKey("SmsRequests");
            Create.AddForeignKey("SmsRequests", "OutpostId", "Outposts");
            Create.AddForeignKey("SmsRequests", "ProductGroupId", "ProductGroups");

            Create.Table("RawSmsReceiveds")
                .WithCommonColumns()
                .WithColumn("Sender").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Content").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Credits").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("OutpostId").AsGuid().Nullable()
                .WithColumn("ParseSucceeded").AsBoolean().Nullable()
                .WithColumn("ParseErrorMessage").AsString(ConstraintUtility.NAME_LENGTH);

            Create.AddForeignKey("RawSmsReceiveds");
            Create.AddForeignKey("RawSmsReceiveds", "OutpostId", "Outposts");
        }
    }
}
