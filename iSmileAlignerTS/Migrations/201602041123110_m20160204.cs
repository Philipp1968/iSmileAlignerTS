namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160204 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceNr = c.Int(nullable: false),
                        ReDate = c.DateTime(nullable: false),
                        CustomerGivenName = c.String(maxLength: 50),
                        CustomerSurName = c.String(maxLength: 50),
                        CustomerEmail = c.String(maxLength: 100),
                        BillingStreet1 = c.String(maxLength: 50),
                        BillingStreet2 = c.String(maxLength: 50),
                        BillingCity = c.String(maxLength: 30),
                        BillingPostcode = c.String(maxLength: 10),
                        BillingCountry = c.String(maxLength: 10),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax0 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax10 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax20 = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.InvoiceNr)
                .Index(t => t.ReDate);
            
            CreateTable(
                "dbo.Journals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceNr = c.Int(nullable: false),
                        LineNr = c.Int(nullable: false),
                        WareNumber = c.Int(nullable: false),
                        Longtext = c.String(maxLength: 100),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(maxLength: 10),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.InvoiceNr)
                .Index(t => t.WareNumber);
            
            AddColumn("dbo.AspNetUsers", "isCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Wares", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Wares", "Longtext", c => c.String(maxLength: 100));
            AlterColumn("dbo.Wares", "Currency", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropIndex("dbo.Journals", new[] { "WareNumber" });
            DropIndex("dbo.Journals", new[] { "InvoiceNr" });
            DropIndex("dbo.Invoices", new[] { "ReDate" });
            DropIndex("dbo.Invoices", new[] { "InvoiceNr" });
            AlterColumn("dbo.Wares", "Currency", c => c.String());
            AlterColumn("dbo.Wares", "Longtext", c => c.String());
            DropColumn("dbo.Wares", "Tax");
            DropColumn("dbo.AspNetUsers", "isCustomer");
            DropTable("dbo.Journals");
            DropTable("dbo.Invoices");
        }
    }
}
