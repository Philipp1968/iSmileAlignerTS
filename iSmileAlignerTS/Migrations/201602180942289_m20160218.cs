namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160218 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "hasFastInternet", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "hasFastInternet");
        }
    }
}
