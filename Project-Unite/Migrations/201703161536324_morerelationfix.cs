namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class morerelationfix : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ForumCategories", name: "ForumCategory_Id", newName: "Parent_Id");
            RenameColumn(table: "dbo.ACLForumPermissions", name: "ForumCategory_Id", newName: "Parent_Id");
            RenameColumn(table: "dbo.ForumTopics", name: "ForumCategory_Id", newName: "Parent_Id");
            RenameColumn(table: "dbo.ForumPosts", name: "ForumTopic_Id", newName: "Parent_Id");
            RenameColumn(table: "dbo.ForumPostEdits", name: "ForumPost_Id", newName: "Parent_Id");
            RenameColumn(table: "dbo.Likes", name: "ForumPost_Id", newName: "Post_Id");
            RenameColumn(table: "dbo.ForumPollVotes", name: "ForumPollOption_Id", newName: "Option_Id");
            RenameColumn(table: "dbo.ForumPollOptions", name: "ForumPoll_Id", newName: "Poll_Id");
            RenameIndex(table: "dbo.ForumCategories", name: "IX_ForumCategory_Id", newName: "IX_Parent_Id");
            RenameIndex(table: "dbo.ACLForumPermissions", name: "IX_ForumCategory_Id", newName: "IX_Parent_Id");
            RenameIndex(table: "dbo.ForumTopics", name: "IX_ForumCategory_Id", newName: "IX_Parent_Id");
            RenameIndex(table: "dbo.ForumPosts", name: "IX_ForumTopic_Id", newName: "IX_Parent_Id");
            RenameIndex(table: "dbo.ForumPostEdits", name: "IX_ForumPost_Id", newName: "IX_Parent_Id");
            RenameIndex(table: "dbo.Likes", name: "IX_ForumPost_Id", newName: "IX_Post_Id");
            RenameIndex(table: "dbo.ForumPollOptions", name: "IX_ForumPoll_Id", newName: "IX_Poll_Id");
            RenameIndex(table: "dbo.ForumPollVotes", name: "IX_ForumPollOption_Id", newName: "IX_Option_Id");
            AddColumn("dbo.ForumPolls", "Parent_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumPolls", "Parent_Id");
            AddForeignKey("dbo.ForumPolls", "Parent_Id", "dbo.ForumTopics", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumPolls", "Parent_Id", "dbo.ForumTopics");
            DropIndex("dbo.ForumPolls", new[] { "Parent_Id" });
            DropColumn("dbo.ForumPolls", "Parent_Id");
            RenameIndex(table: "dbo.ForumPollVotes", name: "IX_Option_Id", newName: "IX_ForumPollOption_Id");
            RenameIndex(table: "dbo.ForumPollOptions", name: "IX_Poll_Id", newName: "IX_ForumPoll_Id");
            RenameIndex(table: "dbo.Likes", name: "IX_Post_Id", newName: "IX_ForumPost_Id");
            RenameIndex(table: "dbo.ForumPostEdits", name: "IX_Parent_Id", newName: "IX_ForumPost_Id");
            RenameIndex(table: "dbo.ForumPosts", name: "IX_Parent_Id", newName: "IX_ForumTopic_Id");
            RenameIndex(table: "dbo.ForumTopics", name: "IX_Parent_Id", newName: "IX_ForumCategory_Id");
            RenameIndex(table: "dbo.ACLForumPermissions", name: "IX_Parent_Id", newName: "IX_ForumCategory_Id");
            RenameIndex(table: "dbo.ForumCategories", name: "IX_Parent_Id", newName: "IX_ForumCategory_Id");
            RenameColumn(table: "dbo.ForumPollOptions", name: "Poll_Id", newName: "ForumPoll_Id");
            RenameColumn(table: "dbo.ForumPollVotes", name: "Option_Id", newName: "ForumPollOption_Id");
            RenameColumn(table: "dbo.Likes", name: "Post_Id", newName: "ForumPost_Id");
            RenameColumn(table: "dbo.ForumPostEdits", name: "Parent_Id", newName: "ForumPost_Id");
            RenameColumn(table: "dbo.ForumPosts", name: "Parent_Id", newName: "ForumTopic_Id");
            RenameColumn(table: "dbo.ForumTopics", name: "Parent_Id", newName: "ForumCategory_Id");
            RenameColumn(table: "dbo.ACLForumPermissions", name: "Parent_Id", newName: "ForumCategory_Id");
            RenameColumn(table: "dbo.ForumCategories", name: "Parent_Id", newName: "ForumCategory_Id");
        }
    }
}
