namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201510291 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseFiles", "FileNum", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CaseFiles", "FileNum");
        }
    }
}
