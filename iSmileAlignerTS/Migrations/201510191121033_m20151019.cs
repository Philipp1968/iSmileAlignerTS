namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20151019 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "CaseNumberStr", c => c.String(maxLength: 20));
            AddColumn("dbo.Cases", "ErstellungsDatum", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "CountryCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "DocShortName", c => c.String());
            CreateIndex("dbo.Cases", "CaseNumberStr");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Cases", new[] { "CaseNumberStr" });
            DropColumn("dbo.AspNetUsers", "DocShortName");
            DropColumn("dbo.AspNetUsers", "CountryCode");
            DropColumn("dbo.Cases", "ErstellungsDatum");
            DropColumn("dbo.Cases", "CaseNumberStr");
        }
    }
}
