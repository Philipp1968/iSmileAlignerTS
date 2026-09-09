namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201602102 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "PaymentNumber", c => c.Long(nullable: false));
            CreateIndex("dbo.Invoices", "PaymentNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Invoices", new[] { "PaymentNumber" });
            DropColumn("dbo.Invoices", "PaymentNumber");
        }
    }
}
