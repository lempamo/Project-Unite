namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cdelete : DbMigration
    {
        public override void Up()
        {
            //Cannot insert the value NULL into column 'Parent_Id', table 'aspnet-Project-Unite-20170315114859.dbo.ForumCategories'; column does not allow nulls. UPDATE fails.
            //The statement has been terminated.


                        DropForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories");
            DropForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories");
            DropForeignKey("dbo.ForumPosts", "Parent_Id", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumPostEdits", "Parent_Id", "dbo.ForumPosts");
            DropForeignKey("dbo.Likes", "Post_Id", "dbo.ForumPosts");
            DropForeignKey("dbo.Likes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ForumPollOptions", "Poll_Id", "dbo.ForumPolls");
            DropForeignKey("dbo.ForumPollVotes", "Option_Id", "dbo.ForumPollOptions");
            DropForeignKey("dbo.ForumPolls", "Parent_Id", "dbo.ForumTopics");
            DropIndex("dbo.ForumCategories", new[] { "Parent_Id" });
            DropIndex("dbo.ACLForumPermissions", new[] { "Parent_Id" });
            DropIndex("dbo.ForumTopics", new[] { "Parent_Id" });
            DropIndex("dbo.ForumPosts", new[] { "Parent_Id" });
            DropIndex("dbo.ForumPostEdits", new[] { "Parent_Id" });
            DropIndex("dbo.Likes", new[] { "Post_Id" });
            DropIndex("dbo.Likes", new[] { "User_Id" });
            DropIndex("dbo.ForumPollOptions", new[] { "Poll_Id" });
            DropIndex("dbo.ForumPolls", new[] { "Parent_Id" });
            DropIndex("dbo.ForumPollVotes", new[] { "Option_Id" });
            AlterColumn("dbo.ForumCategories", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ACLForumPermissions", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumTopics", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumPosts", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumPostEdits", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Likes", "Post_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Likes", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumPollOptions", "Poll_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumPolls", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumPollVotes", "Option_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ForumCategories", "Parent_Id");
            CreateIndex("dbo.ACLForumPermissions", "Parent_Id");
            CreateIndex("dbo.ForumTopics", "Parent_Id");
            CreateIndex("dbo.ForumPosts", "Parent_Id");
            CreateIndex("dbo.ForumPostEdits", "Parent_Id");
            CreateIndex("dbo.Likes", "Post_Id");
            CreateIndex("dbo.Likes", "User_Id");
            CreateIndex("dbo.ForumPollOptions", "Poll_Id");
            CreateIndex("dbo.ForumPolls", "Parent_Id");
            CreateIndex("dbo.ForumPollVotes", "Option_Id");
            AddForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumPosts", "Parent_Id", "dbo.ForumTopics", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumPostEdits", "Parent_Id", "dbo.ForumPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "Post_Id", "dbo.ForumPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumPollOptions", "Poll_Id", "dbo.ForumPolls", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumPollVotes", "Option_Id", "dbo.ForumPollOptions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumPolls", "Parent_Id", "dbo.ForumTopics", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumPolls", "Parent_Id", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumPollVotes", "Option_Id", "dbo.ForumPollOptions");
            DropForeignKey("dbo.ForumPollOptions", "Poll_Id", "dbo.ForumPolls");
            DropForeignKey("dbo.Likes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "Post_Id", "dbo.ForumPosts");
            DropForeignKey("dbo.ForumPostEdits", "Parent_Id", "dbo.ForumPosts");
            DropForeignKey("dbo.ForumPosts", "Parent_Id", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories");
            DropForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories");
            DropIndex("dbo.ForumPollVotes", new[] { "Option_Id" });
            DropIndex("dbo.ForumPolls", new[] { "Parent_Id" });
            DropIndex("dbo.ForumPollOptions", new[] { "Poll_Id" });
            DropIndex("dbo.Likes", new[] { "User_Id" });
            DropIndex("dbo.Likes", new[] { "Post_Id" });
            DropIndex("dbo.ForumPostEdits", new[] { "Parent_Id" });
            DropIndex("dbo.ForumPosts", new[] { "Parent_Id" });
            DropIndex("dbo.ForumTopics", new[] { "Parent_Id" });
            DropIndex("dbo.ACLForumPermissions", new[] { "Parent_Id" });
            DropIndex("dbo.ForumCategories", new[] { "Parent_Id" });
            AlterColumn("dbo.ForumPollVotes", "Option_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPolls", "Parent_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPollOptions", "Poll_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Likes", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Likes", "Post_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPostEdits", "Parent_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPosts", "Parent_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumTopics", "Parent_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ACLForumPermissions", "Parent_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumCategories", "Parent_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumPollVotes", "Option_Id");
            CreateIndex("dbo.ForumPolls", "Parent_Id");
            CreateIndex("dbo.ForumPollOptions", "Poll_Id");
            CreateIndex("dbo.Likes", "User_Id");
            CreateIndex("dbo.Likes", "Post_Id");
            CreateIndex("dbo.ForumPostEdits", "Parent_Id");
            CreateIndex("dbo.ForumPosts", "Parent_Id");
            CreateIndex("dbo.ForumTopics", "Parent_Id");
            CreateIndex("dbo.ACLForumPermissions", "Parent_Id");
            CreateIndex("dbo.ForumCategories", "Parent_Id");
            AddForeignKey("dbo.ForumPolls", "Parent_Id", "dbo.ForumTopics", "Id");
            AddForeignKey("dbo.ForumPollVotes", "Option_Id", "dbo.ForumPollOptions", "Id");
            AddForeignKey("dbo.ForumPollOptions", "Poll_Id", "dbo.ForumPolls", "Id");
            AddForeignKey("dbo.Likes", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Likes", "Post_Id", "dbo.ForumPosts", "Id");
            AddForeignKey("dbo.ForumPostEdits", "Parent_Id", "dbo.ForumPosts", "Id");
            AddForeignKey("dbo.ForumPosts", "Parent_Id", "dbo.ForumTopics", "Id");
            AddForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories", "Id");
            AddForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories", "Id");
        }
    }
}
