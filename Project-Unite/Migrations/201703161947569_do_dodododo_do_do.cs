namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class do_dodododo_do_do : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumTopics", new[] { "Parent_Id", "Parent_Parent" }, "dbo.ForumCategories");
            DropForeignKey("dbo.ACLForumPermissions", new[] { "Parent_Id", "Parent_Parent" }, "dbo.ForumCategories");
            DropForeignKey("dbo.ForumCategories", new[] { "ForumCategory_Id", "ForumCategory_Parent" }, "dbo.ForumCategories");
            DropIndex("dbo.ForumTopics", new[] { "Parent_Id", "Parent_Parent" });
            DropIndex("dbo.ACLForumPermissions", new[] { "Parent_Id", "Parent_Parent" });
            DropIndex("dbo.ForumCategories", new[] { "ForumCategory_Id", "ForumCategory_Parent" });
            DropPrimaryKey("dbo.ForumCategories");
            AlterColumn("dbo.ForumCategories", "Parent", c => c.String());
            DropColumn("dbo.ForumTopics", "Parent_Parent");
            DropColumn("dbo.ACLForumPermissions", "Parent_Parent");
            DropColumn("dbo.ForumCategories", "ForumCategory_Parent");
            AddPrimaryKey("dbo.ForumCategories", "Id");
            CreateIndex("dbo.ForumTopics", "Parent_Id");
            CreateIndex("dbo.ACLForumPermissions", "Parent_Id");
            CreateIndex("dbo.ForumCategories", "ForumCategory_Id");
            AddForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumCategories", "ForumCategory_Id", "dbo.ForumCategories", "Id");
        }
    }
}
