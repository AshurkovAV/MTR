namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixActMeeAndEqma : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FactActEqma", "AccountId", c => c.Int());
            AlterColumn("dbo.FactActMee", "AccountId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FactActMee", "AccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.FactActEqma", "AccountId", c => c.Int(nullable: false));
        }
    }
}
