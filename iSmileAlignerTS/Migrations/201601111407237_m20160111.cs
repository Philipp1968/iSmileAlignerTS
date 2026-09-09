namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160111 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "FlexId", c => c.String(maxLength: 100));
            CreateIndex("dbo.Payments", "FlexId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Payments", new[] { "FlexId" });
            DropColumn("dbo.Payments", "FlexId");
        }
    }
}
