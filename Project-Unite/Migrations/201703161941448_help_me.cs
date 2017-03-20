namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class help_me : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ForumCategories", name: "Parent_Id", newName: "ForumCategory_Id");
            RenameIndex(table: "dbo.ForumCategories", name: "IX_Parent_Id", newName: "IX_ForumCategory_Id");
            AddColumn("dbo.ForumCategories", "Parent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumCategories", "Parent");
            RenameIndex(table: "dbo.ForumCategories", name: "IX_ForumCategory_Id", newName: "IX_Parent_Id");
            RenameColumn(table: "dbo.ForumCategories", name: "ForumCategory_Id", newName: "Parent_Id");
        }
    }
}
