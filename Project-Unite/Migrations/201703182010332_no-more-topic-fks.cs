namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nomoretopicfks : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ForumTopics", "ForumCategory_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id");
            AddForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories", "Id");
        }
    }
}
