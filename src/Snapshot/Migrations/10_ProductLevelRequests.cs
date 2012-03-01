using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(10)]
    public class ProductLevelRequests:Migration
    {
        const string PRODUCT_LEVEL_REQUESTS = "ProductLevelRequests";
        public override void Down()
        {
            Delete.RemoveClientForeignKey(PRODUCT_LEVEL_REQUESTS);
            Delete.RemoveForeignKey(PRODUCT_LEVEL_REQUESTS);
            Delete.RemoveForeignKey(PRODUCT_LEVEL_REQUESTS, "Campaign_FK", "Campaigns");
            Delete.RemoveForeignKey(PRODUCT_LEVEL_REQUESTS, "Schedule_FK", "Schedules");
            Delete.Table(PRODUCT_LEVEL_REQUESTS);
        }

        public override void Up()
        {
            Create.Table(PRODUCT_LEVEL_REQUESTS)
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Campaign_FK").AsGuid()
                .WithColumn("Schedule_FK").AsGuid()
                .WithColumn("ProductGroup_FK").AsGuid()
                .WithColumn("IsStopped").AsBoolean().WithDefaultValue(false)
                .WithColumn("Products").AsBinary(Int32.MaxValue).Nullable()
                ;
            Create.AddClientForeignKey(PRODUCT_LEVEL_REQUESTS);
            Create.AddForeignKey(PRODUCT_LEVEL_REQUESTS);
            Create.AddForeignKey(PRODUCT_LEVEL_REQUESTS, "Campaign_FK", "Campaigns");
            Create.AddForeignKey(PRODUCT_LEVEL_REQUESTS, "Schedule_FK", "Schedules");
        }
    }
}
