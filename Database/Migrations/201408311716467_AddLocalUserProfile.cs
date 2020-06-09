namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocalUserProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.localSettings",
                c => new
                    {
                        SettingsID = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 200),
                        Value = c.String(),
                        Metadata = c.String(),
                    })
                .PrimaryKey(t => t.SettingsID);
            
            CreateTable(
                "dbo.localUserSettings",
                c => new
                    {
                        UserSettingsID = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 200),
                        Value = c.String(),
                        Metadata = c.String(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserSettingsID)
                .ForeignKey("dbo.localUser", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            AddColumn("dbo.FactMedicalEvent", "MoPrice", c => c.Decimal(storeType: "money"));
            AddColumn("dbo.FactMedicalEvent", "MoPaymentStatus", c => c.Int());
            AddColumn("dbo.localUser", "LastName", c => c.String(maxLength: 200));
            AddColumn("dbo.localUser", "FirstName", c => c.String(maxLength: 200));
            AddColumn("dbo.localUser", "Patronymic", c => c.String(maxLength: 200));
            AddColumn("dbo.localUser", "Position", c => c.String(maxLength: 254));
            AddColumn("dbo.localUser", "Phone", c => c.String(maxLength: 200));
            AddColumn("dbo.localUser", "ConfNumber", c => c.String(maxLength: 200));
            DropColumn("dbo.localUser", "ProfileID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.localUser", "ProfileID", c => c.Int(nullable: false));
            DropForeignKey("dbo.localUserSettings", "UserID", "dbo.localUser");
            DropIndex("dbo.localUserSettings", new[] { "UserID" });
            DropColumn("dbo.localUser", "ConfNumber");
            DropColumn("dbo.localUser", "Phone");
            DropColumn("dbo.localUser", "Position");
            DropColumn("dbo.localUser", "Patronymic");
            DropColumn("dbo.localUser", "FirstName");
            DropColumn("dbo.localUser", "LastName");
            DropColumn("dbo.FactMedicalEvent", "MoPaymentStatus");
            DropColumn("dbo.FactMedicalEvent", "MoPrice");
            DropTable("dbo.localUserSettings");
            DropTable("dbo.localSettings");
        }
    }
}
