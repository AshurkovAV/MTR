namespace Medical.DatabaseCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddElmedicineClassifiers1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.globalClinicalExamination",
                c => new
                    {
                        ClinicalExaminationId = c.Int(nullable: false),
                        ServiceCode = c.String(),
                        ProfileId = c.Int(nullable: false),
                        SpecialityId = c.Int(nullable: false),
                        Age = c.Int(nullable: false),
                        AgeMonths = c.Int(nullable: false),
                        Sex = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        IsChildren = c.Boolean(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClinicalExaminationId);
            
            CreateTable(
                "dbo.globalLicense",
                c => new
                    {
                        LicenseId = c.Int(nullable: false),
                        MedicalOrganization = c.String(maxLength: 6),
                        DateBegin = c.DateTime(),
                        DateEnd = c.DateTime(),
                        DateStop = c.DateTime(),
                        LicenseNumber = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.LicenseId);
            
            CreateTable(
                "dbo.globalLicenseEntry",
                c => new
                    {
                        LicenseEntryId = c.Int(nullable: false),
                        AssistanceCondition_id = c.Int(),
                        AssistanceType_Id = c.Int(),
                        License_LicenseId = c.Int(),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.LicenseEntryId)
                .ForeignKey("dbo.V006", t => t.AssistanceCondition_id)
                .ForeignKey("dbo.V008", t => t.AssistanceType_Id)
                .ForeignKey("dbo.globalLicense", t => t.License_LicenseId)
                .ForeignKey("dbo.V002", t => t.Profile_Id)
                .Index(t => t.AssistanceCondition_id)
                .Index(t => t.AssistanceType_Id)
                .Index(t => t.License_LicenseId)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.globalV015EqV002",
                c => new
                    {
                        V015EqV002Id = c.Int(nullable: false),
                        Profile_Id = c.Int(),
                        Speciality_Id = c.Int(),
                    })
                .PrimaryKey(t => t.V015EqV002Id)
                .ForeignKey("dbo.V002", t => t.Profile_Id)
                .ForeignKey("dbo.V015", t => t.Speciality_Id)
                .Index(t => t.Profile_Id)
                .Index(t => t.Speciality_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.globalV015EqV002", "Speciality_Id", "dbo.V015");
            DropForeignKey("dbo.globalV015EqV002", "Profile_Id", "dbo.V002");
            DropForeignKey("dbo.globalLicenseEntry", "Profile_Id", "dbo.V002");
            DropForeignKey("dbo.globalLicenseEntry", "License_LicenseId", "dbo.globalLicense");
            DropForeignKey("dbo.globalLicenseEntry", "AssistanceType_Id", "dbo.V008");
            DropForeignKey("dbo.globalLicenseEntry", "AssistanceCondition_id", "dbo.V006");
            DropIndex("dbo.globalV015EqV002", new[] { "Speciality_Id" });
            DropIndex("dbo.globalV015EqV002", new[] { "Profile_Id" });
            DropIndex("dbo.globalLicenseEntry", new[] { "Profile_Id" });
            DropIndex("dbo.globalLicenseEntry", new[] { "License_LicenseId" });
            DropIndex("dbo.globalLicenseEntry", new[] { "AssistanceType_Id" });
            DropIndex("dbo.globalLicenseEntry", new[] { "AssistanceCondition_id" });
            DropTable("dbo.globalV015EqV002");
            DropTable("dbo.globalLicenseEntry");
            DropTable("dbo.globalLicense");
            DropTable("dbo.globalClinicalExamination");
        }
    }
}
