namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160208 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseCarts", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Invoices", "PaymentId", c => c.Long(nullable: false));
            AddColumn("dbo.Payments", "isPayed", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Invoices", "PaymentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Invoices", new[] { "PaymentId" });
            DropColumn("dbo.Payments", "isPayed");
            DropColumn("dbo.Invoices", "PaymentId");
            DropColumn("dbo.CaseCarts", "Tax");
        }
    }
}
