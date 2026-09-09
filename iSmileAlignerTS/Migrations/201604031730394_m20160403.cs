namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160403 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wares", "PriceDoc", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Wares", "PricePat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Wares", "InShop", c => c.Int(nullable: false));
            CreateIndex("dbo.Wares", "InShop");
            DropColumn("dbo.Wares", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wares", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropIndex("dbo.Wares", new[] { "InShop" });
            DropColumn("dbo.Wares", "InShop");
            DropColumn("dbo.Wares", "PricePat");
            DropColumn("dbo.Wares", "PriceDoc");
        }
    }
}
