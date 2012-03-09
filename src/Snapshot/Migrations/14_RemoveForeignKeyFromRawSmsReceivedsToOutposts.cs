using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(14)]
    public class _14_RemoveForeignKeyFromRawSmsReceivedsToOutposts : Migration
    {
        public override void Down()
        {
            Create.AddForeignKey("RawSmsReceiveds", "OutpostId", "Outposts");
        }

        public override void Up()
        {
            Delete.RemoveForeignKey("RawSmsReceiveds", "OutpostId", "Outposts");
        }
    }
}
