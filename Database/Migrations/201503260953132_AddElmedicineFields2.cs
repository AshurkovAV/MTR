namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddElmedicineFields2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactMedicalEvent", "IdKsg", c => c.Int());
            AddColumn("dbo.FactMedicalEvent", "JobStatus_Id", c => c.Int());
            CreateIndex("dbo.FactMedicalEvent", "JobStatus_Id");
            AddForeignKey("dbo.FactMedicalEvent", "JobStatus_Id", "dbo.F009", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FactMedicalEvent", "JobStatus_Id", "dbo.F009");
            DropIndex("dbo.FactMedicalEvent", new[] { "JobStatus_Id" });
            DropColumn("dbo.FactMedicalEvent", "JobStatus_Id");
            DropColumn("dbo.FactMedicalEvent", "IdKsg");
        }
    }
}
