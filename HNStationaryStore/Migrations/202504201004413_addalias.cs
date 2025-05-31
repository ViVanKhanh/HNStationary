namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addalias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubCategories", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubCategories", "Alias");
        }
    }
}
