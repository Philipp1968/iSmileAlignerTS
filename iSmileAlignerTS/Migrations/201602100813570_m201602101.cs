namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201602101 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseCarts", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Journals", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Journals", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Journals", "UnitPrice");
            DropColumn("dbo.Journals", "Amount");
            DropColumn("dbo.CaseCarts", "UnitPrice");
        }
    }
}
