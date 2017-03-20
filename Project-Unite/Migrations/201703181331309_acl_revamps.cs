namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class acl_revamps : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.ACLForumPermissions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ACLForumPermissions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CategoryId = c.String(),
                        RoleId = c.String(),
                        CanSee = c.Boolean(nullable: false),
                        CanReply = c.Boolean(nullable: false),
                        CanPost = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
