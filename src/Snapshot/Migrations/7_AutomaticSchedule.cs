using System;
using System.Linq;
using FluentMigrator;

namespace Migrations
{
    [Migration(7)]
    public class AutomaticSchedule : Migration
    {

        public override void Down()
        {
            Delete.RemoveForeignKey("RequestReminder");
            Delete.RemoveForeignKey("RequestReminder", "Schedule_FK", "Schedule");
            Delete.Table("RequestReminder");

            Delete.RemoveClientForeignKey("Schedule");
            Delete.RemoveForeignKey("Schedule");
            Delete.Table("Schedule");
        }

        public override void Up()
        {
            Create.Table("Schedule")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("FrequencyType").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("FrequencyValue").AsInt32()
                .WithColumn("StartOn").AsInt32()
                .WithColumn("ScheduleBasis").AsString(ConstraintUtility.NAME_LENGTH);

            Create.AddClientForeignKey("Schedule");
            Create.AddForeignKey("Schedule");

            Create.Table("RequestReminder")
                .WithCommonColumns()
                .WithColumn("PeriodType").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("PeriodValue").AsInt32()
                .WithColumn("Schedule_FK").AsGuid();
            Create.AddForeignKey("RequestReminder", "Schedule_FK", "Schedule");
        }
    }
}
