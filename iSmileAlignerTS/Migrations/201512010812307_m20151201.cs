namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20151201 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cases", "Zeitplan", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cases", "Zeitplan", c => c.String(maxLength: 50));
        }
    }
}
