namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fuck : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ForumCategories", new[] { "Parent_Id" });
            AlterColumn("dbo.ForumCategories", "Parent_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumCategories", "Parent_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ForumCategories", new[] { "Parent_Id" });
            AlterColumn("dbo.ForumCategories", "Parent_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ForumCategories", "Parent_Id");
        }
    }
}
