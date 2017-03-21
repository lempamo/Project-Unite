namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
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

            var adm = new Models.Role
            {
                Id = "administrator",
                ColorHex = "#FF0000",
                Description = "These are the admins of the website - This is a persistent group and cannot be deleted.",
                Priority = context.Roles.Count() + 1,
                Name = "Administrators"
            };
            foreach (var prop in adm.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (prop.Name.StartsWith("Can") && prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(adm, true);
                }
            }
            context.Roles.AddOrUpdate(adm);
            var userRole = new Models.Role
            {
                Name = "Users",
                Id = "user",
                Description = "This is the default role for all new users. This role's priority may not be modified, and this role may not be deleted.",
                Priority = 0,
                ColorHex = "#FFF"
            };
            context.Roles.AddOrUpdate(userRole);
            context.SaveChanges();

        }

    }
}
