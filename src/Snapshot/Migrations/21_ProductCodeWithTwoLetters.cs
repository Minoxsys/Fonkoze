using FluentMigrator;

namespace Migrations
{
    [Migration(21)]
    public class ProductCodeWithTwoLetters : Migration
    {
        public override void Up()
        {
            Alter.Table("Products").AlterColumn("SMSReferenceCode").AsString(5);
        }

        public override void Down()
        {
            Alter.Table("Products").AlterColumn("SMSReferenceCode").AsFixedLengthString(1);
        }
    }
}
