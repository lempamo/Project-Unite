namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forum_backend_base : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForumCategories",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ParentId = c.String(),
                        LinkUrl = c.String(),
                        ForumCategory_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumCategories", t => t.ForumCategory_Id)
                .Index(t => t.ForumCategory_Id);
            
            CreateTable(
                "dbo.ForumPollOptions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PollId = c.String(),
                        Name = c.String(),
                        ForumPoll_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumPolls", t => t.ForumPoll_Id)
                .Index(t => t.ForumPoll_Id);
            
            CreateTable(
                "dbo.ForumPollVotes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PollOptionId = c.String(),
                        UserId = c.String(),
                        ForumPollOption_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumPollOptions", t => t.ForumPollOption_Id)
                .Index(t => t.ForumPollOption_Id);
            
            CreateTable(
                "dbo.ForumPolls",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        TopicId = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AllowMultivote = c.Boolean(nullable: false),
                        AllowVoteChanges = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ForumPosts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AuthorId = c.String(),
                        TopicId = c.String(),
                        Body = c.String(),
                        ForumTopic_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumTopics", t => t.ForumTopic_Id)
                .Index(t => t.ForumTopic_Id);
            
            CreateTable(
                "dbo.ForumTopics",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(),
                        AuthorId = c.String(),
                        CategoryId = c.String(),
                        IsSticky = c.Boolean(nullable: false),
                        IsAnnounce = c.Boolean(nullable: false),
                        IsUnlisted = c.Boolean(nullable: false),
                        IsGlobal = c.Boolean(nullable: false),
                        PollId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumPosts", "ForumTopic_Id", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumPollOptions", "ForumPoll_Id", "dbo.ForumPolls");
            DropForeignKey("dbo.ForumPollVotes", "ForumPollOption_Id", "dbo.ForumPollOptions");
            DropForeignKey("dbo.ForumCategories", "ForumCategory_Id", "dbo.ForumCategories");
            DropIndex("dbo.ForumPosts", new[] { "ForumTopic_Id" });
            DropIndex("dbo.ForumPollVotes", new[] { "ForumPollOption_Id" });
            DropIndex("dbo.ForumPollOptions", new[] { "ForumPoll_Id" });
            DropIndex("dbo.ForumCategories", new[] { "ForumCategory_Id" });
            DropTable("dbo.ForumTopics");
            DropTable("dbo.ForumPosts");
            DropTable("dbo.ForumPolls");
            DropTable("dbo.ForumPollVotes");
            DropTable("dbo.ForumPollOptions");
            DropTable("dbo.ForumCategories");
        }
    }
}
