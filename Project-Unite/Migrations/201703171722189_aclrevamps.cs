namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aclrevamps : DbMigration
    {

        public override void Up()
        {
            AddColumn("dbo.ACLForumPermissions", "CategoryId", c => c.String());
        }
        
        public override void Down()
        {
            AddColumn("dbo.ACLForumPermissions", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.ACLForumPermissions", "CategoryId");
            CreateIndex("dbo.ACLForumPermissions", "Parent_Id");
            AddForeignKey("dbo.ACLForumPermissions", "Parent_Id", "dbo.ForumCategories", "Id", cascadeDelete: true);
        }
    }
}
