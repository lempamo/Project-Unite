namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dislikes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Likes", "LikedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Likes", "Topic", c => c.String(nullable: false));
            AddColumn("dbo.Likes", "IsDislike", c => c.Boolean(nullable: false));
            DropColumn("dbo.ForumTopics", "Votes");
            DropColumn("dbo.Likes", "Post");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Likes", "Post", c => c.String(nullable: false));
            AddColumn("dbo.ForumTopics", "Votes", c => c.Int(nullable: false));
            DropColumn("dbo.Likes", "IsDislike");
            DropColumn("dbo.Likes", "Topic");
            DropColumn("dbo.Likes", "LikedAt");
        }
    }
}
