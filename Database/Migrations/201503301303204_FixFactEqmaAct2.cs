namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixFactEqmaAct2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactActEqma", "WorkPlace", c => c.String(maxLength: 250));
            AddColumn("dbo.FactActEqma", "Address", c => c.String(maxLength: 250));
            AddColumn("dbo.FactActEqma", "OutcomeComments", c => c.String(maxLength: 500));
            AddColumn("dbo.FactActEqma", "DiagnosisComplication", c => c.String(maxLength: 100));
            AddColumn("dbo.FactActEqma", "DiagnosisComplicationComments", c => c.String(maxLength: 254));
            AddColumn("dbo.FactActEqma", "DiagnosisPostmortemComplication", c => c.String(maxLength: 250));
            AlterColumn("dbo.FactActEqma", "Duration", c => c.Decimal(storeType: "money"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FactActEqma", "Duration", c => c.String(maxLength: 254));
            DropColumn("dbo.FactActEqma", "DiagnosisPostmortemComplication");
            DropColumn("dbo.FactActEqma", "DiagnosisComplicationComments");
            DropColumn("dbo.FactActEqma", "DiagnosisComplication");
            DropColumn("dbo.FactActEqma", "OutcomeComments");
            DropColumn("dbo.FactActEqma", "Address");
            DropColumn("dbo.FactActEqma", "WorkPlace");
        }
    }
}
