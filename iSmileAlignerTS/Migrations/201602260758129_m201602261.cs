namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201602261 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Laenders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ISOCode = c.String(maxLength: 5),
                        PLZCode = c.String(maxLength: 5),
                        Land = c.String(maxLength: 100),
                        Zone = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ISOCode)
                .Index(t => t.PLZCode);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Laenders", new[] { "PLZCode" });
            DropIndex("dbo.Laenders", new[] { "ISOCode" });
            DropTable("dbo.Laenders");
        }
    }
}
