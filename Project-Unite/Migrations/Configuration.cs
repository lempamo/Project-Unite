namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Project_Unite.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Project_Unite.Models.ApplicationDbContext";
        }

        protected override void Seed(Project_Unite.Models.ApplicationDbContext context)
        {
            
        }

    }
}
