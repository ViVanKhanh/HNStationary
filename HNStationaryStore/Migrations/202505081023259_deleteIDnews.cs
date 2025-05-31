namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteIDnews : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.News", "AuthorID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.News", "AuthorID", c => c.Int(nullable: false));
        }
    }
}
