using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{

    [Migration(11)]
    public class ExistingRequests : Migration
    {
        public override void Down()
        {
            Delete.RemoveClientForeignKey("RequestRecords");
            Delete.Table("RequestRecords");
        }

        public override void Up()
        {
            Create.Table("RequestRecords")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("CampaignId").AsGuid().Nullable()
                .WithColumn("OutpostId").AsGuid().Nullable()
                .WithColumn("ProductGroupId").AsGuid().Nullable()
                .WithColumn("CampaignName").AsString().Nullable()
                .WithColumn("OutpostName").AsString().Nullable()
                .WithColumn("ProductGroupName").AsString().Nullable()
                .WithColumn("ProductsNo").AsInt32().WithDefaultValue(0)
                .WithColumn("ProductLevelRequestId").AsGuid().Nullable();

            Create.AddClientForeignKey("RequestRecords");
        }
    }
}
