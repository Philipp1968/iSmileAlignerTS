namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20180212 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cases", "PatBirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cases", "PatBirthDate", c => c.DateTime(nullable: false));
        }
    }
}
