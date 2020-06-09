namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddElmedicineFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactMedicalEvent", "RegionalAttribute", c => c.Int());
            AddColumn("dbo.FactMedicalEvent", "HealthGroup", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactMedicalEvent", "HealthGroup");
            DropColumn("dbo.FactMedicalEvent", "RegionalAttribute");
        }
    }
}
