namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThumbNail : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cases", "TreatComment", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cases", "TreatComment", c => c.String(maxLength: 2000));
        }
    }
}
