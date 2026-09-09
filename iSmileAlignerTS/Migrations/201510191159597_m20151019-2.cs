namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201510192 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DaySerials",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Day = c.DateTime(nullable: false),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Day, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DaySerials", new[] { "Day" });
            DropTable("dbo.DaySerials");
        }
    }
}
