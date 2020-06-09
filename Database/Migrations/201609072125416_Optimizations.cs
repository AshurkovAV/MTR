namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Optimizations : DbMigration
    {
        public override void Up()
        {
            CreateIndex("FactMedicalEvent", new string[] { "ExternalId" }, false, "IX_ExternalId");
        }
        
        public override void Down()
        {
            DropIndex("FactMedicalEvent", "IX_ExternalId");
        }
    }
}
