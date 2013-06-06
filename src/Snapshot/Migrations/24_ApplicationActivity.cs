using FluentMigrator;

namespace Migrations
{
    [Migration(24)]
    public class ApplicationActivity : Migration
    {
        public override void Up()
        {
            Create.Table("ApplicationActivities").
                WithCommonColumns().
                WithColumn("Message").AsString(5000).Nullable();

            Create.AddForeignKey("ApplicationActivities");
        }

        public override void Down()
        {
            Delete.RemoveForeignKey("ApplicationActivities");
            Delete.Table("ApplicationActivities");
        }
    }
}
