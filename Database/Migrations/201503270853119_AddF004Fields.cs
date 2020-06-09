namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddF004Fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.F004", "code", c => c.String(nullable: false, maxLength: 7));
            AddColumn("dbo.F004", "speciality", c => c.String(maxLength: 254));
            AddColumn("dbo.F004", "experience", c => c.Int());
            AddColumn("dbo.F004", "date_edit", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.F004", "date_edit");
            DropColumn("dbo.F004", "experience");
            DropColumn("dbo.F004", "speciality");
            DropColumn("dbo.F004", "code");
        }
    }
}
