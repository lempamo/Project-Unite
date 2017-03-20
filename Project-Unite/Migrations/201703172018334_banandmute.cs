namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class banandmute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "JoinedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "LastLogin", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsBanned", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsMuted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "BannedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "MutedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "BannedBy", c => c.String());
            AddColumn("dbo.AspNetUsers", "MutedBy", c => c.String());
            AddColumn("dbo.AspNetUsers", "Interests", c => c.String());
            AddColumn("dbo.AspNetUsers", "Hobbies", c => c.String());
            AddColumn("dbo.AspNetRoles", "CanViewUserInfo", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "CanViewUserInfo");
            DropColumn("dbo.AspNetUsers", "Hobbies");
            DropColumn("dbo.AspNetUsers", "Interests");
            DropColumn("dbo.AspNetUsers", "MutedBy");
            DropColumn("dbo.AspNetUsers", "BannedBy");
            DropColumn("dbo.AspNetUsers", "MutedAt");
            DropColumn("dbo.AspNetUsers", "BannedAt");
            DropColumn("dbo.AspNetUsers", "IsMuted");
            DropColumn("dbo.AspNetUsers", "IsBanned");
            DropColumn("dbo.AspNetUsers", "LastLogin");
            DropColumn("dbo.AspNetUsers", "JoinedAt");
        }
    }
}
