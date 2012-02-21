using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(6)]
    public class _6_AutomaticSchedule : Migration
    {

        public override void Down()
        {
            Delete.RemoveForeignKey("RequestReminder");
            Delete.RemoveForeignKey("RequestReminder", "Schedule_FK", "Schedule");
            Delete.Table("RequestReminder");

            Delete.RemoveForeignKey("Schedule");
            Delete.Table("Schedule");
        }

        public override void Up()
        {
            Create.Table("Schedule")
                .WithCommonColumns()
                .WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("FrequencyType").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("FrequencyValue").AsInt32()
                .WithColumn("StartOn").AsInt32()
                .WithColumn("ScheduleBasis").AsString(ConstraintUtility.NAME_LENGTH);

            Create.Table("RequestReminder")
                .WithCommonColumns()
                .WithColumn("PeriodType").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("PeriodValue").AsInt32()
                .WithColumn("Schedule_FK").AsGuid();
            Create.AddForeignKey("RequestReminder", "Schedule_FK", "Schedule");
        }
    }
}
