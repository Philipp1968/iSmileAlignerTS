namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CaseNumberStrLonger : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Cases", new[] { "CaseNumberStr" });
            AlterColumn("dbo.Cases", "CaseNumberStr", c => c.String(maxLength: 200));
            AlterColumn("dbo.AspNetUsers", "CountryCode", c => c.String(maxLength: 2));
            AlterColumn("dbo.AspNetUsers", "DocShortName", c => c.String(maxLength: 100));
            CreateIndex("dbo.Cases", "CaseNumberStr");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Cases", new[] { "CaseNumberStr" });
            AlterColumn("dbo.AspNetUsers", "DocShortName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "CountryCode", c => c.String());
            AlterColumn("dbo.Cases", "CaseNumberStr", c => c.String(maxLength: 20));
            CreateIndex("dbo.Cases", "CaseNumberStr");
        }
    }
}
