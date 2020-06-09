namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocalF003Fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.localF003", "Phone", c => c.String(maxLength: 40));
            AlterColumn("dbo.localF003", "ShortName", c => c.String(maxLength: 250));
            AlterColumn("dbo.localF003", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.localF003", "Surname", c => c.String(maxLength: 50));
            AlterColumn("dbo.localF003", "Patronymic", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.localF003", "Patronymic", c => c.String(maxLength: 40));
            AlterColumn("dbo.localF003", "Surname", c => c.String(maxLength: 40));
            AlterColumn("dbo.localF003", "Name", c => c.String(maxLength: 40));
            AlterColumn("dbo.localF003", "ShortName", c => c.String(maxLength: 40));
            DropColumn("dbo.localF003", "Phone");
        }
    }
}
