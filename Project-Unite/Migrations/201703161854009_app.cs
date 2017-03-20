namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class app : DbMigration
    {
        public override void Up()
        {
            //Column names in each table must be unique. Column name 'Parent_Id' in table 'dbo.ACLForumPermissions' is specified more than once.
            //Cannot drop the table 'dbo.ACLForumPermissionForumCategories', because it does not exist or you do not have permission.


            DropForeignKey("dbo.ACLForumPermissionForumCategories", "ACLForumPermission_Id", "dbo.ACLForumPermissions");
            DropForeignKey("dbo.ACLForumPermissionForumCategories", "ForumCategory_Id", "dbo.ForumCategories");
            DropIndex("dbo.ACLForumPermissionForumCategories", new[] { "ACLForumPermission_Id" });
            DropIndex("dbo.ACLForumPermissionForumCategories", new[] { "ForumCategory_Id" });
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ACLForumPermissionForumCategories",
                c => new
                    {
                        ACLForumPermission_Id = c.String(nullable: false, maxLength: 128),
                        ForumCategory_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ACLForumPermission_Id, t.ForumCategory_Id });
            
            DropForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories");
            DropIndex("dbo.ACLForumPermissions", new[] { "Parent_Id" });
            DropColumn("dbo.ACLForumPermissions", "Parent_Id");
            CreateIndex("dbo.ACLForumPermissionForumCategories", "ForumCategory_Id");
            CreateIndex("dbo.ACLForumPermissionForumCategories", "ACLForumPermission_Id");
            AddForeignKey("dbo.ACLForumPermissionForumCategories", "ForumCategory_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ACLForumPermissionForumCategories", "ACLForumPermission_Id", "dbo.ACLForumPermissions", "Id", cascadeDelete: true);
        }
    }
}
