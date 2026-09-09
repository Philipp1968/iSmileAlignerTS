namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20151103idx : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.CaseCarts", "CartNumber");
            CreateIndex("dbo.CaseCarts", "CaseNumber");
            CreateIndex("dbo.CaseCarts", "CreateDate");
            CreateIndex("dbo.CaseCarts", "WareNumber");
            CreateIndex("dbo.CaseFiles", "CaseId");
            CreateIndex("dbo.CaseFiles", "Filename");
            CreateIndex("dbo.CaseFiles", "FileNum");
            CreateIndex("dbo.Cases", "CaseNumber");
            CreateIndex("dbo.Cases", "PatFirstname");
            CreateIndex("dbo.Cases", "PatLastName");
            CreateIndex("dbo.Messages", "CaseId");
            CreateIndex("dbo.Payments", "CaseNumber");
            CreateIndex("dbo.Payments", "CaseCart");
            CreateIndex("dbo.Settings", "Name");
            CreateIndex("dbo.Wares", "WareNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Wares", new[] { "WareNumber" });
            DropIndex("dbo.Settings", new[] { "Name" });
            DropIndex("dbo.Payments", new[] { "CaseCart" });
            DropIndex("dbo.Payments", new[] { "CaseNumber" });
            DropIndex("dbo.Messages", new[] { "CaseId" });
            DropIndex("dbo.Cases", new[] { "PatLastName" });
            DropIndex("dbo.Cases", new[] { "PatFirstname" });
            DropIndex("dbo.Cases", new[] { "CaseNumber" });
            DropIndex("dbo.CaseFiles", new[] { "FileNum" });
            DropIndex("dbo.CaseFiles", new[] { "Filename" });
            DropIndex("dbo.CaseFiles", new[] { "CaseId" });
            DropIndex("dbo.CaseCarts", new[] { "WareNumber" });
            DropIndex("dbo.CaseCarts", new[] { "CreateDate" });
            DropIndex("dbo.CaseCarts", new[] { "CaseNumber" });
            DropIndex("dbo.CaseCarts", new[] { "CartNumber" });
        }
    }
}
