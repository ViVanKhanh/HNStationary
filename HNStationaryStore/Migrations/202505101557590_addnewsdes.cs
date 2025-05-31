namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewsdes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "Description", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "Description");
        }
    }
}
