using FluentMigrator;

namespace Migrations
{
    [Migration(18)]
    public class AlertTypes : Migration
    {
        public override void Up()
        {
            Alter.Table("Alerts").AddColumn("AlertType").AsString().Nullable();
            Alter.Table("Alerts").AlterColumn("LastUpdate").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Execute.EmbeddedScript("sqlserver_DeleteColumnFromAlerts_18.sql");
            Alter.Table("Alerts").AlterColumn("LastUpdate").AsDateTime().NotNullable();
        }
    }
}
