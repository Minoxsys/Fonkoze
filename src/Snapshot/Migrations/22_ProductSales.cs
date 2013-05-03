using FluentMigrator;

namespace Migrations
{
    [Migration(22)]
    public class ProductSales : Migration
    {
        const string PRODUCT_SALES = "ProductSales";

        public override void Up()
        {
            Create.Table(PRODUCT_SALES)
              .WithCommonColumns()
              .WithColumn("Outpost_FK").AsGuid()
              .WithColumn("Product_FK").AsGuid()
              .WithColumn("Quantity").AsInt32()
              .WithColumn("ClientIdentifier").AsString(1); //specific requirement, F for Fonkoze Client, N- for not fonkoze
           
            Create.AddForeignKey(PRODUCT_SALES);
        }

        public override void Down()
        {
            Delete.RemoveForeignKey(PRODUCT_SALES);
            Delete.Table(PRODUCT_SALES);

        }
    }
}
