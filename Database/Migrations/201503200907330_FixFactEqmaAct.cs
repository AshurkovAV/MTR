namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixFactEqmaAct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactActEqma", "RefusalPrice", c => c.Decimal(storeType: "money"));
            AlterColumn("dbo.FactEQMA", "ActId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FactEQMA", "ActId", c => c.Int(nullable: false));
            DropColumn("dbo.FactActEqma", "RefusalPrice");
        }
    }
}
