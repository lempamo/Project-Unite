namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jonny___ : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForumTopics", "Parent_Parent", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ACLForumPermissions", "Parent_Parent", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ForumCategories", "ForumCategory_Parent", c => c.String(maxLength: 128));
            AddColumn("dbo.ForumCategories", "ForumCategory_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ForumTopics", "Parent_Id", "dbo.ForumCategories");
            DropForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories");
            DropIndex("dbo.ForumTopics", new[] { "Parent_Id" });
            DropIndex("dbo.ACLForumPermissions", new[] { "Parent_Id" });
            DropPrimaryKey("dbo.ForumCategories");
            AlterColumn("dbo.ForumCategories", "Parent", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ForumCategories", new[] { "Id", "Parent" });
            CreateIndex("dbo.ForumTopics", new[] { "Parent_Id", "Parent_Parent" });
            CreateIndex("dbo.ACLForumPermissions", new[] { "Parent_Id", "Parent_Parent" });
            CreateIndex("dbo.ForumCategories", new[] { "ForumCategory_Id", "ForumCategory_Parent" });
            AddForeignKey("dbo.ForumTopics", new[] { "Parent_Id", "Parent_Parent" }, "dbo.ForumCategories", new[] { "Id", "Parent" }, cascadeDelete: true);
            AddForeignKey("dbo.ACLForumPermissions", new[] { "Parent_Id", "Parent_Parent" }, "dbo.ForumCategories", new[] { "Id", "Parent" }, cascadeDelete: true);
            AddForeignKey("dbo.ForumCategories", new[] { "ForumCategory_Id", "ForumCategory_Parent" }, "dbo.ForumCategories", new[] { "Id", "Parent" });
        }
    }
}
