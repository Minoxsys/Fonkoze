using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(6)]
    public class CampaignAdministration: Migration
    {

        public override void Down()
        {
            Delete.RemoveClientForeignKey("Campaigns");
            Delete.RemoveForeignKey("Campaigns");
            Delete.Table("Campaigns");
        }

        public override void Up()
        {
            Create.Table("Campaigns")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("StartDate").AsDateTime()
                .WithColumn("EndDate").AsDateTime()
                .WithColumn("CreationDate").AsDateTime()
                .WithColumn("Open").AsBoolean()
                .WithColumn("Options").AsBinary()
                ;

            Create.AddClientForeignKey("Campaigns");
            Create.AddForeignKey("Campaigns");
        }
    }
}
