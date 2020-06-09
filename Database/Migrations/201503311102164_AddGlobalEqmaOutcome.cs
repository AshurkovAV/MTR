namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGlobalEqmaOutcome : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.globalEqmaOutcomes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        OutcomeName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.FactActEqma", "Outcome", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FactActEqma", "Outcome", c => c.String(maxLength: 250));
            DropTable("dbo.globalEqmaOutcomes");
        }
    }
}
