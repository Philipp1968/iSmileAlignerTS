namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160927 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "BestelltWieAngegeben", c => c.Int(nullable: false));
            AddColumn("dbo.Cases", "BestelltWieAngegebenKommentar", c => c.String());
            AddColumn("dbo.Cases", "BestelltWieAngegebenDatum", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cases", "BestelltWieAngegebenDatum");
            DropColumn("dbo.Cases", "BestelltWieAngegebenKommentar");
            DropColumn("dbo.Cases", "BestelltWieAngegeben");
        }
    }
}
