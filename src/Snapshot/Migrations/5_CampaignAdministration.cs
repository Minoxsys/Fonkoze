using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(5)]
    public class _5_CampaignAdministration : Migration
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
                .WithColumn("Opened").AsBoolean()
                .WithColumn("Options").AsBinary()
                ;
        }
    }
}
