namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "TypePayment", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "TypePayment", c => c.String(nullable: false));
        }
    }
}
