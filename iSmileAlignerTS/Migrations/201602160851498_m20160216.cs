namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160216 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseCarts", "AddInfoText", c => c.String(maxLength: 100));
            AddColumn("dbo.CaseCarts", "isRequestQuote", c => c.Boolean(nullable: false));
            AddColumn("dbo.Journals", "AddInfoText", c => c.String(maxLength: 100));
            AddColumn("dbo.Wares", "AddInfoText", c => c.String(maxLength: 100));
            AddColumn("dbo.Wares", "isRequestQuote", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Wares", "isRequestQuote");
            DropColumn("dbo.Wares", "AddInfoText");
            DropColumn("dbo.Journals", "AddInfoText");
            DropColumn("dbo.CaseCarts", "isRequestQuote");
            DropColumn("dbo.CaseCarts", "AddInfoText");
        }
    }
}
