namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CasesText10k : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseFiles", "isThumbNail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CaseFiles", "isThumbNail");
        }
    }
}
