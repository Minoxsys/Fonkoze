using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(12)]
    public class InsertWorldCountryRecords: Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            this.IfDatabase("sqlserver").Execute.EmbeddedScript(@"Migrations.Scripts.sqlserver_InsertWorldCountryRecords.sql");
        }
    }
}
