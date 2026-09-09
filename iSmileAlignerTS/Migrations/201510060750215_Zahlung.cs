namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Zahlung : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CaseCarts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CartNumber = c.Int(nullable: false),
                        CaseNumber = c.Int(nullable: false),
                        PaymentNumber = c.Int(nullable: false),
                        isPayed = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WareNumber = c.Long(nullable: false),
                        WareText = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PaymentNumber = c.Int(nullable: false),
                        IsCasePayment = c.Boolean(nullable: false),
                        CaseNumber = c.Int(nullable: false),
                        CaseCart = c.Int(nullable: false),
                        OldState = c.Int(nullable: false),
                        NewOKState = c.Int(nullable: false),
                        UserId = c.String(),
                        BillLine = c.String(),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        WPFStatus = c.Int(nullable: false),
                        StartedTime = c.DateTime(),
                        EventTime = c.DateTime(),
                        FinishedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Wares",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WareNumber = c.Int(nullable: false),
                        Longtext = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Wares");
            DropTable("dbo.Payments");
            DropTable("dbo.CaseCarts");
        }
    }
}
