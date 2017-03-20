namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class acl_unlistedtopics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "CanSeeUnlistedTopics", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "CanSeeUnlistedTopics");
        }
    }
}
