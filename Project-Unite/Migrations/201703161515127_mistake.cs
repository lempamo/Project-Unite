namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mistake : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ForumCategories", new[] { "ForumCategory_Id1" });
            DropIndex("dbo.ForumTopics", new[] { "ForumCategory_Id1" });
            DropIndex("dbo.ForumPosts", new[] { "ForumTopic_Id1" });
            DropIndex("dbo.Likes", new[] { "ForumPost_Id1" });
            DropIndex("dbo.ForumPollOptions", new[] { "ForumPoll_Id1" });
            DropIndex("dbo.ForumPollVotes", new[] { "ForumPollOption_Id1" });
            DropColumn("dbo.ForumCategories", "ForumCategory_Id");
            DropColumn("dbo.ForumTopics", "ForumCategory_Id");
            DropColumn("dbo.ForumPosts", "ForumTopic_Id");
            DropColumn("dbo.Likes", "ForumPost_Id");
            DropColumn("dbo.ForumPollOptions", "ForumPoll_Id");
            DropColumn("dbo.ForumPollVotes", "ForumPollOption_Id");
            RenameColumn(table: "dbo.ForumCategories", name: "ForumCategory_Id1", newName: "ForumCategory_Id");
            RenameColumn(table: "dbo.ForumTopics", name: "ForumCategory_Id1", newName: "ForumCategory_Id");
            RenameColumn(table: "dbo.ForumPosts", name: "ForumTopic_Id1", newName: "ForumTopic_Id");
            RenameColumn(table: "dbo.Likes", name: "ForumPost_Id1", newName: "ForumPost_Id");
            RenameColumn(table: "dbo.ForumPollVotes", name: "ForumPollOption_Id1", newName: "ForumPollOption_Id");
            RenameColumn(table: "dbo.ForumPollOptions", name: "ForumPoll_Id1", newName: "ForumPoll_Id");
            AlterColumn("dbo.ForumCategories", "ForumCategory_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPosts", "ForumTopic_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Likes", "ForumPost_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPollOptions", "ForumPoll_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumPollVotes", "ForumPollOption_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumCategories", "ForumCategory_Id");
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id");
            CreateIndex("dbo.ForumPosts", "ForumTopic_Id");
            CreateIndex("dbo.Likes", "ForumPost_Id");
            CreateIndex("dbo.ForumPollOptions", "ForumPoll_Id");
            CreateIndex("dbo.ForumPollVotes", "ForumPollOption_Id");
            DropColumn("dbo.ForumPolls", "ForumTopic_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForumPolls", "ForumTopic_Id", c => c.String());
            DropIndex("dbo.ForumPollVotes", new[] { "ForumPollOption_Id" });
            DropIndex("dbo.ForumPollOptions", new[] { "ForumPoll_Id" });
            DropIndex("dbo.Likes", new[] { "ForumPost_Id" });
            DropIndex("dbo.ForumPosts", new[] { "ForumTopic_Id" });
            DropIndex("dbo.ForumTopics", new[] { "ForumCategory_Id" });
            DropIndex("dbo.ForumCategories", new[] { "ForumCategory_Id" });
            AlterColumn("dbo.ForumPollVotes", "ForumPollOption_Id", c => c.String());
            AlterColumn("dbo.ForumPollOptions", "ForumPoll_Id", c => c.String());
            AlterColumn("dbo.Likes", "ForumPost_Id", c => c.String());
            AlterColumn("dbo.ForumPosts", "ForumTopic_Id", c => c.String());
            AlterColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String());
            AlterColumn("dbo.ForumCategories", "ForumCategory_Id", c => c.String());
            RenameColumn(table: "dbo.ForumPollOptions", name: "ForumPoll_Id", newName: "ForumPoll_Id1");
            RenameColumn(table: "dbo.ForumPollVotes", name: "ForumPollOption_Id", newName: "ForumPollOption_Id1");
            RenameColumn(table: "dbo.Likes", name: "ForumPost_Id", newName: "ForumPost_Id1");
            RenameColumn(table: "dbo.ForumPosts", name: "ForumTopic_Id", newName: "ForumTopic_Id1");
            RenameColumn(table: "dbo.ForumTopics", name: "ForumCategory_Id", newName: "ForumCategory_Id1");
            RenameColumn(table: "dbo.ForumCategories", name: "ForumCategory_Id", newName: "ForumCategory_Id1");
            AddColumn("dbo.ForumPollVotes", "ForumPollOption_Id", c => c.String());
            AddColumn("dbo.ForumPollOptions", "ForumPoll_Id", c => c.String());
            AddColumn("dbo.Likes", "ForumPost_Id", c => c.String());
            AddColumn("dbo.ForumPosts", "ForumTopic_Id", c => c.String());
            AddColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String());
            AddColumn("dbo.ForumCategories", "ForumCategory_Id", c => c.String());
            CreateIndex("dbo.ForumPollVotes", "ForumPollOption_Id1");
            CreateIndex("dbo.ForumPollOptions", "ForumPoll_Id1");
            CreateIndex("dbo.Likes", "ForumPost_Id1");
            CreateIndex("dbo.ForumPosts", "ForumTopic_Id1");
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id1");
            CreateIndex("dbo.ForumCategories", "ForumCategory_Id1");
        }
    }
}
