namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class votingsystemforums : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForumPostEdits",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        EditReason = c.String(),
                        PreviousState = c.String(),
                        EditedAt = c.DateTime(nullable: false),
                        ForumPost_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumPosts", t => t.ForumPost_Id)
                .Index(t => t.ForumPost_Id);
            
            CreateTable(
                "dbo.Likes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Post_Id = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumPosts", t => t.Post_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.ForumTopics", "Votes", c => c.Int(nullable: false));
            AddColumn("dbo.ForumTopics", "StartedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.ForumPosts", "PostedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetRoles", "CanDeleteForumCategories", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Likes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "Post_Id", "dbo.ForumPosts");
            DropForeignKey("dbo.ForumPostEdits", "ForumPost_Id", "dbo.ForumPosts");
            DropIndex("dbo.Likes", new[] { "User_Id" });
            DropIndex("dbo.Likes", new[] { "Post_Id" });
            DropIndex("dbo.ForumPostEdits", new[] { "ForumPost_Id" });
            DropColumn("dbo.AspNetRoles", "CanDeleteForumCategories");
            DropColumn("dbo.ForumPosts", "PostedAt");
            DropColumn("dbo.ForumTopics", "StartedAt");
            DropColumn("dbo.ForumTopics", "Votes");
            DropTable("dbo.Likes");
            DropTable("dbo.ForumPostEdits");
        }
    }
}
