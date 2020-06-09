namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFactProcessing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FactProcessing",
                c => new
                    {
                        ProcessingId = c.Int(nullable: false, identity: true),
                        Weight = c.Int(),
                        IsEnable = c.Boolean(),
                        DateBegin = c.DateTime(),
                        DateEnd = c.DateTime(),
                        Query = c.String(),
                        Description = c.String(maxLength: 254),
                        Comments = c.String(maxLength: 254),
                        Name = c.String(maxLength: 100),
                        ProcessingType_ProcessingTypeId = c.Int(),
                        Scope_ScopeID = c.Int(),
                        Version_VersionID = c.Int(),
                    })
                .PrimaryKey(t => t.ProcessingId)
                .ForeignKey("dbo.globalProcessingType", t => t.ProcessingType_ProcessingTypeId)
                .ForeignKey("dbo.globalScope", t => t.Scope_ScopeID)
                .ForeignKey("dbo.globalVersion", t => t.Version_VersionID)
                .Index(t => t.ProcessingType_ProcessingTypeId)
                .Index(t => t.Scope_ScopeID)
                .Index(t => t.Version_VersionID);
            
            CreateTable(
                "dbo.globalProcessingType",
                c => new
                    {
                        ProcessingTypeId = c.Int(nullable: false),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ProcessingTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FactProcessing", "Version_VersionID", "dbo.globalVersion");
            DropForeignKey("dbo.FactProcessing", "Scope_ScopeID", "dbo.globalScope");
            DropForeignKey("dbo.FactProcessing", "ProcessingType_ProcessingTypeId", "dbo.globalProcessingType");
            DropIndex("dbo.FactProcessing", new[] { "Version_VersionID" });
            DropIndex("dbo.FactProcessing", new[] { "Scope_ScopeID" });
            DropIndex("dbo.FactProcessing", new[] { "ProcessingType_ProcessingTypeId" });
            DropTable("dbo.globalProcessingType");
            DropTable("dbo.FactProcessing");
        }
    }
}
