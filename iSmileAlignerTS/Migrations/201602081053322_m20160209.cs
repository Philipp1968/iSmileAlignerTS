namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160209 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseCarts", "UserId", c => c.String(maxLength: 200));
            CreateIndex("dbo.CaseCarts", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CaseCarts", new[] { "UserId" });
            DropColumn("dbo.CaseCarts", "UserId");
        }
    }
}
