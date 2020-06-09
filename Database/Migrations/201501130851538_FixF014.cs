using Microsoft.SqlServer.Server;

namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixF014 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.F014", "Osn", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.F014", "Comments", c => c.String(maxLength: 150));
            
        }
        
        public override void Down()
        {
            AlterColumn("dbo.F014", "Comments", c => c.String(maxLength: 100));
            AlterColumn("dbo.F014", "Osn", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
