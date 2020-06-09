namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateActMee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactActMee", "RefusalPrice", c => c.Decimal(storeType: "money"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactActMee", "RefusalPrice");
        }
    }
}
