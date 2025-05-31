namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateNews : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "Image", c => c.String(maxLength: 255));
            AddColumn("dbo.News", "CreateDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.News", "ImageURL");
        }
        
        public override void Down()
        {
            AddColumn("dbo.News", "ImageURL", c => c.String(maxLength: 255));
            DropColumn("dbo.News", "CreateDate");
            DropColumn("dbo.News", "Image");
        }
    }
}
