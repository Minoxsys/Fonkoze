using FluentMigrator;

namespace Migrations
{
    [Migration(19)]
    public class UserEntityPhoneNumber : Migration
    {
        public override void Up()
        {
            Alter.Table("Users").AddColumn("PhoneNumber").AsString().Nullable();
        }

        public override void Down()
        {
            Execute.EmbeddedScript("sqlserver_DeleteColumnFromUsers_19.sql");
        }
    }
}