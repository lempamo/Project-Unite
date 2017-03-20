namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class running_out_of_names : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories");
            DropIndex("dbo.ForumTopics", new[] { "Parent_Id" });
            RenameColumn(table: "dbo.ForumTopics", name: "Parent_Id", newName: "ForumCategory_Id");
            AddColumn("dbo.ForumTopics", "Parent", c => c.String(nullable: false));
            AlterColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumTopics", "ForumCategory_Id");
            AddForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumTopics", "ForumCategory_Id", "dbo.ForumCategories");
            DropIndex("dbo.ForumTopics", new[] { "ForumCategory_Id" });
            AlterColumn("dbo.ForumTopics", "ForumCategory_Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.ForumTopics", "Parent");
            RenameColumn(table: "dbo.ForumTopics", name: "ForumCategory_Id", newName: "Parent_Id");
            CreateIndex("dbo.ForumTopics", "Parent_Id");
            AddForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
        }
    }
}
