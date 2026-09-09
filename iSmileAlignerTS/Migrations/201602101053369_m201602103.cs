namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m201602103 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "Currency", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "Currency");
        }
    }
}
