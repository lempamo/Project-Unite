namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class moreaclstuff : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ACLForumPermissions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(),
                        CanSee = c.Boolean(nullable: false),
                        CanReply = c.Boolean(nullable: false),
                        CanPost = c.Boolean(nullable: false),
                        ForumCategory_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumCategories", t => t.ForumCategory_Id)
                .Index(t => t.ForumCategory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ACLForumPermissions", "ForumCategory_Id", "dbo.ForumCategories");
            DropIndex("dbo.ACLForumPermissions", new[] { "ForumCategory_Id" });
            DropTable("dbo.ACLForumPermissions");
        }
    }
}
