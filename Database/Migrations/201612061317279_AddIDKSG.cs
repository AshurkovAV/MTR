namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIDKSG : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactMedicalServices", "IDKSG", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactMedicalServices", "IDKSG");
        }
    }
}
