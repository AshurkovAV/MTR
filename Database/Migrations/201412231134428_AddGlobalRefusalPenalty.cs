namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGlobalRefusalPenalty : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.globalRefusalPenalty",
                c => new
                    {
                        RefusalPenaltyId = c.Int(nullable: false, identity: true),
                        Reason = c.String(maxLength: 20),
                        Percent = c.Int(nullable: false),
                        Penalty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decrease = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Comments = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.RefusalPenaltyId);
            
            AlterColumn("dbo.F014", "Kod", c => c.String(maxLength: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.F014", "Kod", c => c.String(maxLength: 3));
            DropTable("dbo.globalRefusalPenalty");
        }
    }
}
