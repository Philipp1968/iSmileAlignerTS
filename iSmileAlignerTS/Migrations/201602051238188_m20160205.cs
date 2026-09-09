namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160205 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Invoices", new[] { "InvoiceNr" });
            CreateIndex("dbo.Invoices", "InvoiceNr", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Invoices", new[] { "InvoiceNr" });
            CreateIndex("dbo.Invoices", "InvoiceNr");
        }
    }
}
