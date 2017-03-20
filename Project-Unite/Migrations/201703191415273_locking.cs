namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumTopics", "IsLocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumTopics", "IsLocked");
        }
    }
}
