namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class topic_discriminator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumTopics", "Discriminator", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumTopics", "Discriminator");
        }
    }
}
