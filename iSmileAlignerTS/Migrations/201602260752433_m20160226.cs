namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160226 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Country", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Country", c => c.String(maxLength: 10));
        }
    }
}
