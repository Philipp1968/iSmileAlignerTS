namespace iSmileAlignerTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20160518 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Cases", new[] { "PatFirstname" });
            DropIndex("dbo.Cases", new[] { "PatLastName" });
            AddColumn("dbo.Cases", "Step01Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step02Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step03Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step04Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step05Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step06Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step07Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step08Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step09Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step10Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step11Kommentar", c => c.String(maxLength: 200));
            AddColumn("dbo.Cases", "Step12Kommentar", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "PatFirstname", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "PatLastName", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "PatAddress", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "PatCity", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step01OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step01UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step02OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step02UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step03OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step03UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step04OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step04UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step05OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step05UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step06OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step06UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step07OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step07UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step08OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step08UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step09OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step09UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step10OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step10UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step11OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step11UK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step12OK", c => c.String(maxLength: 200));
            AlterColumn("dbo.Cases", "Step12UK", c => c.String(maxLength: 200));
            CreateIndex("dbo.Cases", "PatFirstname");
            CreateIndex("dbo.Cases", "PatLastName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Cases", new[] { "PatLastName" });
            DropIndex("dbo.Cases", new[] { "PatFirstname" });
            AlterColumn("dbo.Cases", "Step12UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step12OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step11UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step11OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step10UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step10OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step09UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step09OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step08UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step08OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step07UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step07OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step06UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step06OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step05UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step05OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step04UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step04OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step03UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step03OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step02UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step02OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step01UK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "Step01OK", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "PatCity", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "PatAddress", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "PatLastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Cases", "PatFirstname", c => c.String(maxLength: 50));
            DropColumn("dbo.Cases", "Step12Kommentar");
            DropColumn("dbo.Cases", "Step11Kommentar");
            DropColumn("dbo.Cases", "Step10Kommentar");
            DropColumn("dbo.Cases", "Step09Kommentar");
            DropColumn("dbo.Cases", "Step08Kommentar");
            DropColumn("dbo.Cases", "Step07Kommentar");
            DropColumn("dbo.Cases", "Step06Kommentar");
            DropColumn("dbo.Cases", "Step05Kommentar");
            DropColumn("dbo.Cases", "Step04Kommentar");
            DropColumn("dbo.Cases", "Step03Kommentar");
            DropColumn("dbo.Cases", "Step02Kommentar");
            DropColumn("dbo.Cases", "Step01Kommentar");
            CreateIndex("dbo.Cases", "PatLastName");
            CreateIndex("dbo.Cases", "PatFirstname");
        }
    }
}
