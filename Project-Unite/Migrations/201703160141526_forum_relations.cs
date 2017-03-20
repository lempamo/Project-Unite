namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forum_relations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumCategories", "Name", c => c.String());
            AddColumn("dbo.ForumCategories", "Description", c => c.String());
            AddColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id");
            AddForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories");
            DropIndex("dbo.ForumTopics", new[] { "ForumCategory_Id" });
            DropColumn("dbo.ForumTopics", "ForumCategory_Id");
            DropColumn("dbo.ForumCategories", "Description");
            DropColumn("dbo.ForumCategories", "Name");
        }
    }
}
