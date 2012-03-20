using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(13)]
    public class EmailRequests : Migration
    {
        public override void Down()
        {
            Delete.RemoveForeignKey("EmailRequests");
            Delete.RemoveClientForeignKey("EmailRequests");

            Delete.Table("EmailRequests");
        }

        public override void Up()
        {
            Create.Table("EmailRequests")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Date").AsDate()
                .WithColumn("OutpostId").AsGuid()
                .WithColumn("ProductGroupId").AsGuid();

            Create.AddClientForeignKey("EmailRequests");
            Create.AddForeignKey("EmailRequests");
        }
    }
}
