namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201602106 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "TotalNet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "TotalNet");
        }
    }
}
