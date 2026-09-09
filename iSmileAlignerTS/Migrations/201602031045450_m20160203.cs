namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m20160203 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "ResultCode", c => c.String(maxLength: 20));
            AddColumn("dbo.Payments", "ResultCode1", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "ResultCode2", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "ResultCode3", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "ResultCodeMsg", c => c.String(maxLength: 200));
            AddColumn("dbo.Payments", "PrepareCheckout", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "RetrieveCheckout", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "RefundCheckout", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "RefundCheckout");
            DropColumn("dbo.Payments", "RetrieveCheckout");
            DropColumn("dbo.Payments", "PrepareCheckout");
            DropColumn("dbo.Payments", "ResultCodeMsg");
            DropColumn("dbo.Payments", "ResultCode3");
            DropColumn("dbo.Payments", "ResultCode2");
            DropColumn("dbo.Payments", "ResultCode1");
            DropColumn("dbo.Payments", "ResultCode");
        }
    }
}
