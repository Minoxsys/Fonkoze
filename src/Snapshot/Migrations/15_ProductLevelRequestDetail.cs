using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(15)]
    public class ProductLevelRequestDetailMigration:Migration
    {
        const string TABLE = "ProductLevelRequestDetails";
        public override void Down()
        {
            Delete.RemoveForeignKey(TABLE);
            Delete.Table(TABLE);
        }

        public override void Up()
        {
            Create.Table(TABLE)
                .WithCommonColumns()
                .WithColumn("ProductLevelRequestId").AsGuid()
                .WithColumn("ProductGroupName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("RequestMessage").AsString(ConstraintUtility.DESCRIPTION_LENGTH)
                .WithColumn("OutpostName").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Method").AsString(ConstraintUtility.NAME_LENGTH);

            Create.AddForeignKey(TABLE);
        }
    }
}
