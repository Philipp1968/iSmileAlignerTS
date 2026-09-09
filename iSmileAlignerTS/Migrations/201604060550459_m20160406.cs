namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160406 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "paysInvoices", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "paysInvoices");
        }
    }
}
