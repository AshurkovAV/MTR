namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixFactEqmaAct3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactActEqma", "Anamnesis", c => c.String());
            AddColumn("dbo.FactActEqma", "AnamnesisError", c => c.String());
            AddColumn("dbo.FactActEqma", "Diagnosis", c => c.String());
            AddColumn("dbo.FactActEqma", "DiagnosisError", c => c.String());
            AddColumn("dbo.FactActEqma", "Cure", c => c.String());
            AddColumn("dbo.FactActEqma", "CureError", c => c.String());
            AddColumn("dbo.FactActEqma", "Continuity", c => c.String());
            AddColumn("dbo.FactActEqma", "ContinuityError", c => c.String());
            AddColumn("dbo.FactActEqma", "FinalConclusion", c => c.String());
            AddColumn("dbo.FactActEqma", "SignificantError", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactActEqma", "SignificantError");
            DropColumn("dbo.FactActEqma", "FinalConclusion");
            DropColumn("dbo.FactActEqma", "ContinuityError");
            DropColumn("dbo.FactActEqma", "Continuity");
            DropColumn("dbo.FactActEqma", "CureError");
            DropColumn("dbo.FactActEqma", "Cure");
            DropColumn("dbo.FactActEqma", "DiagnosisError");
            DropColumn("dbo.FactActEqma", "Diagnosis");
            DropColumn("dbo.FactActEqma", "AnamnesisError");
            DropColumn("dbo.FactActEqma", "Anamnesis");
        }
    }
}
