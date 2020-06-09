namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEventType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactMedicalEvent", "EventType", c => c.Int());
            AddColumn("dbo.FactMedicalEvent", "Flag", c => c.Long());
            AddColumn("dbo.FactMedicalServices", "IsServiceBeforeEvent", c => c.Boolean());
            AddColumn("dbo.FactMedicalServices", "IsServiceRefuse", c => c.Boolean());
            AddColumn("dbo.FactMedicalServices", "Flag", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactMedicalServices", "Flag");
            DropColumn("dbo.FactMedicalServices", "IsServiceRefuse");
            DropColumn("dbo.FactMedicalServices", "IsServiceBeforeEvent");
            DropColumn("dbo.FactMedicalEvent", "Flag");
            DropColumn("dbo.FactMedicalEvent", "EventType");
        }
    }
}
