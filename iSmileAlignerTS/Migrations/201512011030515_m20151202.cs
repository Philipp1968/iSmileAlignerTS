namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20151202 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "PrivatKommentar", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cases", "PrivatKommentar");
        }
    }
}
