namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddElmedicineClassifiers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.globalMedicalEventType",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.globalRegionalAttribute",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.globalRegionalAttribute");
            DropTable("dbo.globalMedicalEventType");
        }
    }
}
