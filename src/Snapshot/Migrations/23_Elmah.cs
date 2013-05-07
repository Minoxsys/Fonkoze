using FluentMigrator;

namespace Migrations
{
    [Migration(23)]
    public class Elmah : Migration
    {
        public override void Up()
        {
            IfDatabase("sqlserver").Execute.EmbeddedScript(@"ELMAH-1.2-db-SQLServer.sql");
            IfDatabase("sqlserver").Execute.EmbeddedScript(@"ELMAH_TrimLog.sql");
        }

        public override void Down()
        {
            Delete.Table("ELMAH_Error");
        }
    }
}
