using FluentMigrator;

namespace Migrations
{
    [Migration(19)]
    public class DistrictManagersAndPhoneNumbers : Migration
    {
        public override void Up()
        {
            Alter.Table("Users").AddColumn("PhoneNumber").AsString().Nullable();
            Alter.Table("Districts").AddColumn("DistrictManager_FK").AsGuid().Nullable();
            Create.AddForeignKey("Districts", "DistrictManager_FK");
        }

        public override void Down()
        {
            Delete.RemoveForeignKey("Districts", "DistrictManager_FK");
            Execute.EmbeddedScript("sqlserver_DeleteColumnFromUsers_19.sql");

        }
    }
}