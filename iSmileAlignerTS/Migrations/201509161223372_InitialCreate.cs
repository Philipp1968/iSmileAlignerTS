namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CaseFiles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CaseId = c.Long(nullable: false),
                        Filename = c.String(maxLength: 200),
                        ContentType = c.String(maxLength: 100),
                        ContentLength = c.Int(nullable: false),
                        Content = c.Binary(),
                        FileDate = c.DateTime(),
                        FileType = c.Int(nullable: false),
                        isDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cases",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CaseNumber = c.Int(nullable: false),
                        DoctorId = c.String(),
                        CaseState = c.Int(nullable: false),
                        PatSalutation = c.String(maxLength: 20),
                        PatTitel = c.String(maxLength: 20),
                        PatFirstname = c.String(maxLength: 50),
                        PatLastName = c.String(maxLength: 50),
                        PatSex = c.String(maxLength: 2),
                        PatBirthDate = c.DateTime(nullable: false),
                        PatAddress = c.String(maxLength: 50),
                        PatZIP = c.String(maxLength: 12),
                        PatCity = c.String(maxLength: 50),
                        PatCountry = c.String(maxLength: 10),
                        PatPhone = c.String(maxLength: 20),
                        PatMobilPhone = c.String(maxLength: 20),
                        PatEmail = c.String(maxLength: 100),
                        Tooth11 = c.Boolean(nullable: false),
                        Tooth12 = c.Boolean(nullable: false),
                        Tooth13 = c.Boolean(nullable: false),
                        Tooth14 = c.Boolean(nullable: false),
                        Tooth15 = c.Boolean(nullable: false),
                        Tooth16 = c.Boolean(nullable: false),
                        Tooth17 = c.Boolean(nullable: false),
                        Tooth18 = c.Boolean(nullable: false),
                        Tooth21 = c.Boolean(nullable: false),
                        Tooth22 = c.Boolean(nullable: false),
                        Tooth23 = c.Boolean(nullable: false),
                        Tooth24 = c.Boolean(nullable: false),
                        Tooth25 = c.Boolean(nullable: false),
                        Tooth26 = c.Boolean(nullable: false),
                        Tooth27 = c.Boolean(nullable: false),
                        Tooth28 = c.Boolean(nullable: false),
                        Tooth31 = c.Boolean(nullable: false),
                        Tooth32 = c.Boolean(nullable: false),
                        Tooth33 = c.Boolean(nullable: false),
                        Tooth34 = c.Boolean(nullable: false),
                        Tooth35 = c.Boolean(nullable: false),
                        Tooth36 = c.Boolean(nullable: false),
                        Tooth37 = c.Boolean(nullable: false),
                        Tooth38 = c.Boolean(nullable: false),
                        Tooth41 = c.Boolean(nullable: false),
                        Tooth42 = c.Boolean(nullable: false),
                        Tooth43 = c.Boolean(nullable: false),
                        Tooth44 = c.Boolean(nullable: false),
                        Tooth45 = c.Boolean(nullable: false),
                        Tooth46 = c.Boolean(nullable: false),
                        Tooth47 = c.Boolean(nullable: false),
                        Tooth48 = c.Boolean(nullable: false),
                        TreatComment = c.String(maxLength: 2000),
                        Stripping = c.Int(nullable: false),
                        AddTreatClassII = c.Boolean(nullable: false),
                        AddTreatSuspender = c.Boolean(nullable: false),
                        AddTreatExtract = c.Boolean(nullable: false),
                        AddTreatRetainer = c.Boolean(nullable: false),
                        AddTreatRail = c.Boolean(nullable: false),
                        AddTreatExtrusion = c.Boolean(nullable: false),
                        AddTreatButtons = c.Boolean(nullable: false),
                        AddTreatPontic = c.Boolean(nullable: false),
                        AddTreatWire = c.Boolean(nullable: false),
                        Step01OK = c.String(maxLength: 50),
                        Step01UK = c.String(maxLength: 50),
                        Step02OK = c.String(maxLength: 50),
                        Step02UK = c.String(maxLength: 50),
                        Step03OK = c.String(maxLength: 50),
                        Step03UK = c.String(maxLength: 50),
                        Step04OK = c.String(maxLength: 50),
                        Step04UK = c.String(maxLength: 50),
                        Step05OK = c.String(maxLength: 50),
                        Step05UK = c.String(maxLength: 50),
                        Step06OK = c.String(maxLength: 50),
                        Step06UK = c.String(maxLength: 50),
                        Step07OK = c.String(maxLength: 50),
                        Step07UK = c.String(maxLength: 50),
                        Step08OK = c.String(maxLength: 50),
                        Step08UK = c.String(maxLength: 50),
                        Step09OK = c.String(maxLength: 50),
                        Step09UK = c.String(maxLength: 50),
                        Step10OK = c.String(maxLength: 50),
                        Step10UK = c.String(maxLength: 50),
                        Step11OK = c.String(maxLength: 50),
                        Step11UK = c.String(maxLength: 50),
                        Step12OK = c.String(maxLength: 50),
                        Step12UK = c.String(maxLength: 50),
                        Zeitplan = c.String(maxLength: 50),
                        Payment1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Payment1Date = c.DateTime(),
                        Payment2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Payment2Date = c.DateTime(),
                        Accepted = c.Boolean(nullable: false),
                        AcceptedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromUserEmail = c.String(maxLength: 100),
                        ToUserEmail = c.String(maxLength: 100),
                        CaseId = c.Long(nullable: false),
                        Msg = c.String(maxLength: 1000),
                        MessageDate = c.DateTime(nullable: false),
                        isReplied = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Value = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Titel = c.String(maxLength: 20),
                        Salutation = c.String(maxLength: 20),
                        Firstname = c.String(maxLength: 50),
                        Lastname = c.String(maxLength: 50),
                        BirthDate = c.DateTime(nullable: false),
                        MedicalField = c.String(maxLength: 20),
                        Praxis = c.String(maxLength: 50),
                        Address = c.String(maxLength: 50),
                        ZIP = c.String(maxLength: 12),
                        City = c.String(maxLength: 50),
                        Country = c.String(maxLength: 10),
                        Phone = c.String(maxLength: 20),
                        MobilPhone = c.String(maxLength: 20),
                        Fax = c.String(maxLength: 20),
                        WebURL = c.String(maxLength: 100),
                        isAdmin = c.Boolean(nullable: false),
                        isDoctor = c.Boolean(nullable: false),
                        needVerify = c.Boolean(nullable: false),
                        needVerifyDate = c.DateTime(),
                        isVerified = c.Boolean(nullable: false),
                        isVerfiedDate = c.DateTime(),
                        isDeactivated = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Settings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.Cases");
            DropTable("dbo.CaseFiles");
        }
    }
}
