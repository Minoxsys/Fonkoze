using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(5)]
    public class WorkItemMigration :Migration
    {
        public override void Down()
        {
            Delete.Table("WorkItems");
        }

        public override void Up()
        {
            Create.Table("WorkItems").WithCommonColumns()
                .WithColumn("WorkItemId").AsInt64().Identity()
                .WithColumn("JobName").AsFixedLengthString(64)
                .WithColumn("WorkerId").AsFixedLengthString(64)
                .WithColumn("Started").AsDateTime().NotNullable()
                .WithColumn("Completed").AsDateTime().Nullable()
                .WithColumn("ExceptionInfo").AsString(8000).Nullable();
        }
    }
}
