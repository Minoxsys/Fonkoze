using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(17)]
    public class Insert3Roles: Migration
    {
        public override void Down()
        {
            this.IfDatabase("sqlserver").Execute.EmbeddedScript(@"Migrations.Scripts.sqlserver_Insert3Roles_Down_17.sql");
        }

        public override void Up()
        {
            this.IfDatabase("sqlserver").Execute.EmbeddedScript(@"Migrations.Scripts.sqlserver_Insert3Roles_17.sql");
        }
    }
}
