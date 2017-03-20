namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class more_fucking_acl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForumPermissions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CategoryId = c.String(nullable: false),
                        RoleId = c.String(nullable: false),
                        Permissions = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ForumPermissions");
        }
    }
}
