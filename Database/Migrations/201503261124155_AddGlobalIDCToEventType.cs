namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGlobalIDCToEventType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.globalIDCToEventType",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        IDC = c.String(nullable: false),
                        EventType_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.globalMedicalEventType", t => t.EventType_ID)
                .Index(t => t.EventType_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.globalIDCToEventType", "EventType_ID", "dbo.globalMedicalEventType");
            DropIndex("dbo.globalIDCToEventType", new[] { "EventType_ID" });
            DropTable("dbo.globalIDCToEventType");
        }
    }
}
