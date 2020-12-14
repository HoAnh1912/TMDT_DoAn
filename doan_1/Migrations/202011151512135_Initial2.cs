namespace doan_1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "UserName", c => c.String());
            AddColumn("dbo.Comments", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Image");
            DropColumn("dbo.Comments", "UserName");
        }
    }
}
