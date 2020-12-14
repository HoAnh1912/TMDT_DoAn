namespace doan_1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        MaBill = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false),
                        TongHoaDon = c.Single(nullable: false),
                        OrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaBill)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentID = c.Int(nullable: false, identity: true),
                        NoiDungBL = c.String(),
                        BookID = c.Int(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.Books", t => t.BookID, cascadeDelete: true)
                .Index(t => t.BookID);
            
            AddColumn("dbo.Orders", "FullName", c => c.String());
            AddColumn("dbo.Orders", "PhoneNumber", c => c.String());
            AddColumn("dbo.Orders", "AddressDelivery", c => c.String());
            AddColumn("dbo.Orders", "ThanhToan", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "BookID", "dbo.Books");
            DropForeignKey("dbo.Bills", "OrderID", "dbo.Orders");
            DropIndex("dbo.Comments", new[] { "BookID" });
            DropIndex("dbo.Bills", new[] { "OrderID" });
            DropColumn("dbo.Orders", "ThanhToan");
            DropColumn("dbo.Orders", "AddressDelivery");
            DropColumn("dbo.Orders", "PhoneNumber");
            DropColumn("dbo.Orders", "FullName");
            DropTable("dbo.Comments");
            DropTable("dbo.Bills");
        }
    }
}
