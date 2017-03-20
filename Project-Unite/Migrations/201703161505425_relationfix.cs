namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForumCategories", "ForumCategory_Id", "dbo.ForumCategories");
            DropForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories");
            DropForeignKey("dbo.ForumPosts", "ForumTopic_Id", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumPollVotes", "ForumPollOption_Id", "dbo.ForumPollOptions");
            DropForeignKey("dbo.ForumPollOptions", "ForumPoll_Id", "dbo.ForumPolls");
            DropIndex("dbo.ForumCategories", new[] { "ForumCategory_Id" });
            DropIndex("dbo.ForumTopics", new[] { "ForumCategory_Id" });
            DropIndex("dbo.ForumPosts", new[] { "ForumTopic_Id" });
            DropIndex("dbo.ForumPollOptions", new[] { "ForumPoll_Id" });
            DropIndex("dbo.ForumPollVotes", new[] { "ForumPollOption_Id" });
            RenameColumn(table: "dbo.Likes", name: "Post_Id", newName: "ForumPost_Id1");
            RenameIndex(table: "dbo.Likes", name: "IX_Post_Id", newName: "IX_ForumPost_Id1");
            AddColumn("dbo.ForumCategories", "ForumCategory_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.ForumTopics", "ForumCategory_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.ForumPosts", "ForumTopic_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.Likes", "ForumPost_Id", c => c.String());
            AddColumn("dbo.ForumPollOptions", "ForumPoll_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.ForumPollVotes", "ForumPollOption_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.ForumPolls", "ForumTopic_Id", c => c.String());
            AlterColumn("dbo.ForumCategories", "ForumCategory_Id", c => c.String());
            AlterColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String());
            AlterColumn("dbo.ForumPosts", "ForumTopic_Id", c => c.String());
            AlterColumn("dbo.ForumPollOptions", "ForumPoll_Id", c => c.String());
            AlterColumn("dbo.ForumPollVotes", "ForumPollOption_Id", c => c.String());
            CreateIndex("dbo.ForumCategories", "ForumCategory_Id1");
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id1");
            CreateIndex("dbo.ForumPosts", "ForumTopic_Id1");
            CreateIndex("dbo.ForumPollOptions", "ForumPoll_Id1");
            CreateIndex("dbo.ForumPollVotes", "ForumPollOption_Id1");
            AddForeignKey("dbo.ForumCategories", "ForumCategory_Id1", "dbo.ForumCategories", "Id");
            AddForeignKey("dbo.ForumTopics", "ForumCategory_Id1", "dbo.ForumCategories", "Id");
            AddForeignKey("dbo.ForumPosts", "ForumTopic_Id1", "dbo.ForumTopics", "Id");
            AddForeignKey("dbo.ForumPollVotes", "ForumPollOption_Id1", "dbo.ForumPollOptions", "Id");
            AddForeignKey("dbo.ForumPollOptions", "ForumPoll_Id1", "dbo.ForumPolls", "Id");
            DropColumn("dbo.ForumCategories", "ParentId");
            DropColumn("dbo.ForumTopics", "CategoryId");
            DropColumn("dbo.ForumPosts", "TopicId");
            DropColumn("dbo.ForumPollOptions", "PollId");
            DropColumn("dbo.ForumPollVotes", "PollOptionId");
            DropColumn("dbo.ForumPolls", "TopicId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForumPolls", "TopicId", c => c.String());
            AddColumn("dbo.ForumPollVotes", "PollOptionId", c => c.String());
            AddColumn("dbo.ForumPollOptions", "PollId", c => c.String());
            AddColumn("dbo.ForumPosts", "TopicId", c => c.String());
            AddColumn("dbo.ForumTopics", "CategoryId", c => c.String());
            AddColumn("dbo.ForumCategories", "ParentId", c => c.String());
            DropForeignKey("dbo.ForumPollOptions", "ForumPoll_Id1", "dbo.ForumPolls");
            DropForeignKey("dbo.ForumPollVotes", "ForumPollOption_Id1", "dbo.ForumPollOptions");
            DropForeignKey("dbo.ForumPosts", "ForumTopic_Id1", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumTopics", "ForumCategory_Id1", "dbo.ForumCategories");
            DropForeignKey("dbo.ForumCategories", "ForumCategory_Id1", "dbo.ForumCategories");
            DropIndex("dbo.ForumPollVotes", new[] { "ForumPollOption_Id1" });
            DropIndex("dbo.ForumPollOptions", new[] { "ForumPoll_Id1" });
            DropIndex("dbo.ForumPosts", new[] { "ForumTopic_Id1" });
            DropIndex("dbo.ForumTopics", new[] { "ForumCategory_Id1" });
            DropIndex("dbo.ForumCategories", new[] { "ForumCategory_Id1" });
            AlterColumn("dbo.ForumPollVotes", "ForumPollOption_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPollOptions", "ForumPoll_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPosts", "ForumTopic_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumCategories", "ForumCategory_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.ForumPolls", "ForumTopic_Id");
            DropColumn("dbo.ForumPollVotes", "ForumPollOption_Id1");
            DropColumn("dbo.ForumPollOptions", "ForumPoll_Id1");
            DropColumn("dbo.Likes", "ForumPost_Id");
            DropColumn("dbo.ForumPosts", "ForumTopic_Id1");
            DropColumn("dbo.ForumTopics", "ForumCategory_Id1");
            DropColumn("dbo.ForumCategories", "ForumCategory_Id1");
            RenameIndex(table: "dbo.Likes", name: "IX_ForumPost_Id1", newName: "IX_Post_Id");
            RenameColumn(table: "dbo.Likes", name: "ForumPost_Id1", newName: "Post_Id");
            CreateIndex("dbo.ForumPollVotes", "ForumPollOption_Id");
            CreateIndex("dbo.ForumPollOptions", "ForumPoll_Id");
            CreateIndex("dbo.ForumPosts", "ForumTopic_Id");
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id");
            CreateIndex("dbo.ForumCategories", "ForumCategory_Id");
            AddForeignKey("dbo.ForumPollOptions", "ForumPoll_Id", "dbo.ForumPolls", "Id");
            AddForeignKey("dbo.ForumPollVotes", "ForumPollOption_Id", "dbo.ForumPollOptions", "Id");
            AddForeignKey("dbo.ForumPosts", "ForumTopic_Id", "dbo.ForumTopics", "Id");
            AddForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories", "Id");
            AddForeignKey("dbo.ForumCategories", "ForumCategory_Id", "dbo.ForumCategories", "Id");
        }
    }
}
