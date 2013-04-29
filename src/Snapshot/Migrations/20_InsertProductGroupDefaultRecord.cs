using FluentMigrator;

namespace Migrations
{
    [Migration(20)]
    public class InsertProductGroupDefaultRecord : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            this.IfDatabase("sqlserver").Execute.EmbeddedScript(@"Migrations.Scripts.sqlserver_InsertProductGroupDefaultRecord_20.sql");
        }
    }
}
