using FluentMigrator;

namespace Migrations
{
    [Migration(18)]
    public class AlertTypes : Migration
    {
        public override void Up()
        {
            Alter.Table("Alerts").AddColumn("AlertType").AsString().Nullable();
        }

        public override void Down()
        {
            Execute.EmbeddedScript("sqlserver_DeleteColumnFromAlerts_18.sql");
        }
    }
}
