namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20171122attachment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "AddTreatAttachment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cases", "AddTreatAttachment");
        }
    }
}
