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
            Delete.RemoveForeignKey("RequestReminders");
            Delete.RemoveForeignKey("RequestReminders", "Schedule_FK", "Schedules");
            Delete.Table("RequestReminders");

            Delete.RemoveClientForeignKey("Schedules");
            Delete.RemoveForeignKey("Schedules");
            Delete.Table("Schedules");
        }

        public override void Up()
        {
            Create.Table("Schedules")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("FrequencyType").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("FrequencyValue").AsInt32().Nullable()
                .WithColumn("StartOn").AsInt32().Nullable()
                .WithColumn("ScheduleBasis").AsString(ConstraintUtility.NAME_LENGTH);

            Create.AddClientForeignKey("Schedules");
            Create.AddForeignKey("Schedules");

            Create.Table("RequestReminders")
                .WithCommonColumns()
                .WithColumn("PeriodType").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("PeriodValue").AsInt32().Nullable()
                .WithColumn("Schedule_FK").AsGuid();

            Create.AddForeignKey("RequestReminders");
            Create.AddForeignKey("RequestReminders", "Schedule_FK", "Schedules");
        }
    }
}
