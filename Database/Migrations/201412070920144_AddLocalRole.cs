namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocalRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.localRole",
                c => new
                    {
                        RoleID = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleID);
            
            AddColumn("dbo.localUser", "RoleID_RoleID", c => c.Int());
            CreateIndex("dbo.localUser", "RoleID_RoleID");
            AddForeignKey("dbo.localUser", "RoleID_RoleID", "dbo.localRole", "RoleID");
            DropColumn("dbo.localUser", "RoleID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.localUser", "RoleID", c => c.Int(nullable: false));
            DropForeignKey("dbo.localUser", "RoleID_RoleID", "dbo.localRole");
            DropIndex("dbo.localUser", new[] { "RoleID_RoleID" });
            DropColumn("dbo.localUser", "RoleID_RoleID");
            DropTable("dbo.localRole");
        }
    }
}
