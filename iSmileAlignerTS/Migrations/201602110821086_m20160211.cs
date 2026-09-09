namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160211 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "CaseNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Invoices", "CaseNumberStr", c => c.String(maxLength: 200));
            AddColumn("dbo.Invoices", "CustomerTitel", c => c.String(maxLength: 20));
            AddColumn("dbo.Invoices", "CustomerSalutation", c => c.String(maxLength: 20));
            CreateIndex("dbo.Invoices", "CaseNumber");
            CreateIndex("dbo.Invoices", "CaseNumberStr");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Invoices", new[] { "CaseNumberStr" });
            DropIndex("dbo.Invoices", new[] { "CaseNumber" });
            DropColumn("dbo.Invoices", "CustomerSalutation");
            DropColumn("dbo.Invoices", "CustomerTitel");
            DropColumn("dbo.Invoices", "CaseNumberStr");
            DropColumn("dbo.Invoices", "CaseNumber");
        }
    }
}
