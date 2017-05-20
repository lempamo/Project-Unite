using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project_Unite.Models
{
    public class UserFollow
    {
        public string Id { get; set; }
        public string Follower { get; set; }
        public string Followed { get; set; }
    }

    public class UploadImageViewModel
    {
        [Required(ErrorMessage = "Please select an image to upload.")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string GroupId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
            
        }

        public int Pong_HighestLevel { get; set; }
        public int Pong_HighestCodepointsCashout { get; set; }


        #region Privacy

        public bool ShowFollowers { get; set; }
        public bool ShowFollowed { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPostAndTopicCounts { get; set; }
        public bool ShowJoinDate { get; set; }
        public bool EmailOnNotifications { get; set; }

        #endregion


        public ForumPost[] UnreadPosts
        {
            get
            {
                var db = new ApplicationDbContext();


                var posts = db.ForumPosts.Where(x => db.ForumTopics.FirstOrDefault(z=>z.Id==x.Parent).IsUnlisted == false && db.ReadPosts.FirstOrDefault(y => y.UserId == this.Id && y.PostId == x.Id) == null);
                return posts.ToArray();
            }
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

        public int StoryPosition { get; set; }
        public string Language { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int Revision { get; set; }

        public ShiftoriumUpgrade[] Upgrades
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.ShiftoriumUpgrades.Where(x => x.UserId == this.Id).ToArray();
            }
        }

        public bool IsPatreon { get; set; }
        
        public int ShiftnetSubscription { get; set; }

        public int LastMonthPaid { get; set; }
        public Story[] Stories
        {
            get
            {
                return new ApplicationDbContext().Stories.Where(x => x.UserId == this.Id).ToArray();
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

        [Required(AllowEmptyStrings = false, ErrorMessage ="You must enter a display name.")]
        [MinLength(5, ErrorMessage ="Your display name must be at least 5 characters long.")]
        public string DisplayName { get; set; }

        public string FullName { get; set; }

        public string Website { get; set; }
        public string YoutubeUrl { get; set; }
        public string SystemName { get; set; }

        [AllowHtml]
        public string Interests { get; set; }

        [AllowHtml]
        public string Hobbies { get; set; }
        

        public UserPost[] Posts
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.UserPosts.Where(x => x.UserId == this.Id).ToArray();
            }
        }

        public Notification[] Notifications
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.Notifications.Where(x => x.UserId == this.Id).ToArray();
            }
        }

        public int UnreadNotifications
        {
            get
            {
                return Notifications.Where(x => x.IsRead == false).Count();
            }
        }

        public UserFollow[] Followed
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.Follows.Where(x => x.Follower == this.Id).ToArray();
            }
        }

        public UserFollow[] Followers
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.Follows.Where(x => x.Followed == this.Id).ToArray();
            }
        }
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

        public DbSet<WikiPage> WikiPages { get; set; }
        public DbSet<WikiCategory> WikiCategories { get; set; }

        public void DeleteObject(object obj)
        {
            ((IObjectContextAdapter)this).ObjectContext.DeleteObject(obj);
        }

        

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestEntry> ContestEntries { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<BugTag> BugTags { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<ReadPost> ReadPosts { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<DatabaseBackup> Backups { get; set; }
        public DbSet<AssetBackup> AssetBackups { get; set; }
        public DbSet<Avatar> UserAvatars { get; set; }
        public DbSet<Skin> Skins { get; set; }
        public DbSet<Configuration> Configs { get; set; }
        public DbSet<ShiftoriumUpgrade> ShiftoriumUpgrades { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserFollow> Follows { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<ForumPostEdit> ForumPostEdits { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<BannedIP> BannedIPs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public System.Data.Entity.DbSet<Project_Unite.Models.Role> IdentityRoles { get; set; }
        public DbSet<ForumCategory> ForumCategories { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumPoll> ForumPolls { get; set; }
        public DbSet<ForumPollOption> ForumPollOptions { get; set; }
        public DbSet<ForumPollVote> ForumPollVotes { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<OAuthToken> OAuthTokens { get; set; }
        public DbSet<Group> Groups { get; set; }
    }

    public class OAuthToken
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AppName { get; set; }
        public string AppDescription { get; set; }
        public string Version { get; set; }
    }
    
    public class ReadPost
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
    }

    public class UserPost
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        [MaxLength(1000, ErrorMessage ="Your post can't have more than 1000 characters.")]
        [AllowHtml]
        [MinLength(20, ErrorMessage ="To prevent spam, you must have at least 20 characters in your post.")]
        public string PostContents { get; set; }

        public DateTime PostedAt { get; set; }

        public Like[] Likes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(l => l.Topic == this.Id).Where(x => x.IsDislike == false).ToArray();
            }
        }

        public Like[] Dislikes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(l => l.Topic == this.Id).Where(x => x.IsDislike == true).ToArray();
            }
        }

    }

    public class ShiftoriumUpgrade
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string EngineUpgradeId { get; set; }
    }

    public class Story
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string EngineStoryId { get; set; }
    }

    public class Avatar
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class DatabaseBackup
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string DownloadUrl { get; set; }
    }

    public class AssetBackup
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string DownloadUrl { get; set; }
    }
}