namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rolepriority : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Priority", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Priority");
        }
    }
}
