namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160529 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerCases",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CasesId = c.Long(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CasesId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerCases", new[] { "UserId" });
            DropIndex("dbo.CustomerCases", new[] { "CasesId" });
            DropTable("dbo.CustomerCases");
        }
    }
}
