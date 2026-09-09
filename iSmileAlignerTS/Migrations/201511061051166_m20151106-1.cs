namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201511061 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cases", "RowVersion");
        }
    }
}
