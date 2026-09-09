namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160210 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Payments", "UserId", c => c.String(maxLength: 200));
            CreateIndex("dbo.Payments", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Payments", new[] { "UserId" });
            AlterColumn("dbo.Payments", "UserId", c => c.String());
        }
    }
}
