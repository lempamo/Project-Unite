using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project_Unite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
            
        }

        public Role HighestRole
        {
            get
            {
                var roleList = new List<Role>();
                foreach (var role in this.Roles)
                {
                    roleList.Add(new ApplicationDbContext().Roles.First(r => r.Id == role.RoleId) as Role);

                }
                return roleList.OrderByDescending(x => x.Priority).First();
            }
        }

        public string LastKnownIPAddress { get; set; }

        public DateTime JoinedAt { get; set; }
        public DateTime LastLogin { get; set; }
        
        public bool IsBanned { get; set; }
        public bool IsMuted { get; set; }

        public DateTime BannedAt { get; set; }
        public DateTime MutedAt { get; set; }
        public string BannedBy { get; set; }
        public string MutedBy { get; set; }

        public int PostCount
        {
            get
            {
                using(var db = new ApplicationDbContext())
                {
                    return db.ForumPosts.Where(x=>x.AuthorId == this.Id).Count();
                }
            }
        }

        public int TopicCount
        {
            get
            {
                using (var db = new ApplicationDbContext())
                {
                    return db.ForumTopics.Where(x => x.AuthorId == this.Id).Count();
                }
            }
        }

        public long Codepoints { get; set; }

        [AllowHtml]
        public string Bio { get; set; }

        public string BannerUrl { get; set; }
        public string AvatarUrl { get; set; }

        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Website { get; set; }
        public string YoutubeUrl { get; set; }
        public string SystemName { get; set; }
        
        public string Interests { get; set; }
        public string Hobbies { get; set; }
        

        
    }

    public class BannedIP
    {
        public string Id { get; set; }
        public string Address { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            
        }

        public void DeleteObject(object obj)
        {
            ((IObjectContextAdapter)this).ObjectContext.DeleteObject(obj);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<ForumPostEdit> ForumPostEdits { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<ForumPermission> ForumPermissions { get; set; }
        public DbSet<BannedIP> BannedIPs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public System.Data.Entity.DbSet<Project_Unite.Models.Role> IdentityRoles { get; set; }
        public DbSet<ForumCategory> ForumCategories { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumPoll> ForumPolls { get; set; }
        public DbSet<ForumPollOption> ForumPollOptions { get; set; }
        public DbSet<ForumPollVote> ForumPollVotes { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        
    }
}