namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160405 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "IsJustDelivery", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Invoices", "IsJustDelivery");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Invoices", new[] { "IsJustDelivery" });
            DropColumn("dbo.Invoices", "IsJustDelivery");
        }
    }
}
