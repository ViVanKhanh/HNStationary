namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtypePayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TypePayment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "TypePayment");
        }
    }
}
