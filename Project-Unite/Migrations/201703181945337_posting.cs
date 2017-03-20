namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumPosts", "Parent", c => c.String(nullable: false));
            AddColumn("dbo.ForumPostEdits", "Parent", c => c.String(nullable: false));
            AddColumn("dbo.Likes", "User", c => c.String(nullable: false));
            AddColumn("dbo.Likes", "Post", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.Likes", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Likes", "Post_Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ForumPostEdits", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ForumPosts", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Likes", "Post");
            DropColumn("dbo.Likes", "User");
            DropColumn("dbo.ForumPostEdits", "Parent");
            DropColumn("dbo.ForumPosts", "Parent");
            CreateIndex("dbo.Likes", "User_Id");
            CreateIndex("dbo.Likes", "Post_Id");
            CreateIndex("dbo.ForumPostEdits", "Parent_Id");
            CreateIndex("dbo.ForumPosts", "Parent_Id");
            AddForeignKey("dbo.ForumPosts", "Parent_Id", "dbo.ForumTopics", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "Post_Id", "dbo.ForumPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumPostEdits", "Parent_Id", "dbo.ForumPosts", "Id", cascadeDelete: true);
        }
    }
}
