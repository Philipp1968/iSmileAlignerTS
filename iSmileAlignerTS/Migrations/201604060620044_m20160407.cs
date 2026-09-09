namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160407 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "shopperMerchantDescriptor", c => c.String(maxLength: 50));
            AddColumn("dbo.Payments", "shopperMerchantInvId", c => c.String(maxLength: 50));
            AddColumn("dbo.Payments", "InvoiceDeliveryId", c => c.String(maxLength: 50));
            CreateIndex("dbo.Payments", "shopperMerchantDescriptor");
            CreateIndex("dbo.Payments", "shopperMerchantInvId");
            CreateIndex("dbo.Payments", "InvoiceDeliveryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Payments", new[] { "InvoiceDeliveryId" });
            DropIndex("dbo.Payments", new[] { "shopperMerchantInvId" });
            DropIndex("dbo.Payments", new[] { "shopperMerchantDescriptor" });
            DropColumn("dbo.Payments", "InvoiceDeliveryId");
            DropColumn("dbo.Payments", "shopperMerchantInvId");
            DropColumn("dbo.Payments", "shopperMerchantDescriptor");
        }
    }
}
