using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
	[Migration(2)]
	public class OutpostAdministration : Migration
	{
		public override void Down()
		{
			Delete.RemoveForeignKey("Clients");

			Delete.RemoveForeignKey("Countries");
			Delete.RemoveClientForeignKey("Countries");

			Delete.RemoveForeignKey("Regions");
			Delete.RemoveClientForeignKey("Regions");
			Delete.RemoveForeignKey("Regions", "Country_FK", "Countries");

			Delete.RemoveForeignKey("Districts");
			Delete.RemoveClientForeignKey("Districts");
			Delete.RemoveForeignKey("Districts", "Region_FK", "Regions");

			Delete.RemoveForeignKey("Outposts");
			Delete.RemoveClientForeignKey("Outposts");
			Delete.RemoveForeignKey("Outposts", "Country_FK", "Countries");
			Delete.RemoveForeignKey("Outposts", "Region_FK", "Regions");
			Delete.RemoveForeignKey("Outposts", "District_FK", "Districts");
			Delete.RemoveForeignKey("Outposts", "Warehouse_FK", "Outposts");

			Delete.RemoveClientForeignKey("Contacts");
			Delete.RemoveForeignKey("Contacts", "Outpost_FK", "Outposts");

			Delete.Table("WorldCountryRecords");

			Delete.Table("Countries");
			Delete.Table("Regions");
			Delete.Table("Districts");

			Delete.Table("Contacts");
			Delete.Table("Outposts");

			Delete.Table("Clients");
		}

		public override void Up()
		{
			Create.Table("Clients")
				.WithCommonColumns()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH);

			Create.AddForeignKey("Clients");

			Create.Table("WorldCountryRecords")
				.WithCommonColumns()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("ISOCode").AsString(3)
				.WithColumn("PhonePrefix").AsString(9);

			Create.Table("Countries")
				.WithCommonColumns()
				.WithClientColumn()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("ISOCode").AsString(3)
				.WithColumn("PhonePrefix").AsString(9);
			Create.AddForeignKey("Countries");
			Create.AddClientForeignKey("Countries");
			
			Create.Table("Regions")
				.WithCommonColumns()
				.WithClientColumn()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("Coordinates").AsString(ConstraintUtility.COORDINATES_LENGTH).Nullable()
				.WithColumn("Country_FK").AsGuid();
			Create.AddForeignKey("Regions");
			Create.AddForeignKey("Regions", "Country_FK", "Countries");
			Create.AddClientForeignKey("Regions");            
            
			Create.Table("Districts")
				.WithCommonColumns()
				.WithClientColumn()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("Region_FK").AsGuid()
				;
			Create.AddForeignKey("Districts");
			Create.AddForeignKey("Districts", "Region_FK", "Regions");
			Create.AddClientForeignKey("Districts");
				

			Create.Table("Outposts")
				.WithCommonColumns()
				.WithClientColumn()
				.WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
				.WithColumn("Coordinates").AsString(ConstraintUtility.COORDINATES_LENGTH).Nullable()
				.WithColumn("Longitude").AsString(ConstraintUtility.COORDINATES_LENGTH).Nullable()
				.WithColumn("Latitude").AsString(ConstraintUtility.COORDINATES_LENGTH).Nullable()
				.WithColumn("IsWarehouse").AsBoolean()
				.WithColumn("DetailMethod").AsString(255).Nullable()
				.WithColumn("Country_FK").AsGuid()
				.WithColumn("Warehouse_FK").AsGuid().Nullable()
				.WithColumn("Region_FK").AsGuid()
				.WithColumn("District_FK").AsGuid()
				;
			Create.AddForeignKey("Outposts");
			Create.AddForeignKey("Outposts", "Country_FK", "Countries");
			Create.AddForeignKey("Outposts", "Region_FK", "Regions");
			Create.AddForeignKey("Outposts", "District_FK", "Districts");
			Create.AddForeignKey("Outposts", "Warehouse_FK", "Outposts");
			Create.AddClientForeignKey("Outposts");

			Create.Table("Contacts")
				.WithCommonColumns()
				.WithClientColumn()
				.WithColumn("ContactType").AsString(15).Nullable()
				.WithColumn("ContactDetail").AsString(100).Nullable()
                .WithColumn("IsMainContact").AsBoolean()
				.WithColumn("Outpost_FK").AsGuid();

			Create.AddClientForeignKey("Contacts");
			Create.AddForeignKey("Contacts", "Outpost_FK", "Outposts");
		}

	}
}
