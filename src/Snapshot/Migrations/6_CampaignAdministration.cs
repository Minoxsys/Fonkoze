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
            Delete.RemoveClientForeignKey("Campaign");
            Delete.RemoveForeignKey("Campaign");
            Delete.Table("Campaign");
        }

        public override void Up()
        {
            Create.Table("Campaign")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("StartDate").AsDateTime()
                .WithColumn("EndDate").AsDateTime()
                .WithColumn("CreationDate").AsDateTime()
                .WithColumn("Open").AsBoolean()
                .WithColumn("Options").AsBinary()
                ;  
        }
    }
}
