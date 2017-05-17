using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class ForumCategory
    {
        [Key]
        [Column(Order = 0)]
        public string Id { get; set; }

        public virtual ForumCategory[] Children
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.ForumCategories.Where(x => x.Parent == this.Id).ToArray();
            }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string LinkUrl { get; set; }

        public ForumTopic[] Topics
        {
            get
            {
                return new ApplicationDbContext().ForumTopics.Where(x => x.Parent == this.Id).ToArray();
            }
        }

        public virtual string Parent { get; set; }

        public int AdminPermission { get; set; }
        public int DeveloperPermission { get; set; }
        public int ModeratorPermission { get; set; }
        public int MemberPermission { get; set; }

        public bool VisibleToGuests { get; set; }
    }

    public class ForumPost
    {
        public string Id { get; set; }
        
        public string Subject { get; set; }

        [Required]
        public string Parent { get; set; }

        public string AuthorId { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTime PostedAt { get; set; }

        public virtual ForumPostEdit[] EditHistory
        {
            get
            {
                return new ApplicationDbContext().ForumPostEdits.Where(e => e.Parent == this.Id).ToArray();
            }
        }
    }

    public class ForumPostEdit
    {
        public string Id { get; set; }
        [Required]
        public string Parent { get; set; }

        public string UserId { get; set; }
        public string EditReason { get; set; }
        [AllowHtml]
        public string PreviousState { get; set; }
        public DateTime EditedAt { get; set; }
    }

    public class ForumTopic
    {
        public string Id { get; set; }
        [Required]
        public virtual string Parent { get; set; }

        public Like[] Likes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(l => l.Topic == this.Id).Where(x=>x.IsDislike==false).ToArray();
            }
        }

        public Like[] Dislikes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(l => l.Topic == this.Id).Where(x => x.IsDislike == true).ToArray();
            }
        }


        public string Discriminator { get; set; }

        public bool IsLocked { get; set; }

        public int Priority
        {
            get
            {
                int priority = 0;
                if (IsSticky)
                    priority = 1;
                if (IsAnnounce)
                    priority = 2;
                if (IsSticky && IsAnnounce)
                    priority = 3;
                return priority;
            }
        }

        public DateTime StartedAt { get; set; }
        public string Subject { get; set; }
        public bool ShouldShow
        {
            get
            {
                if (IsUnlisted == true)
                    return HttpContext.Current.User?.Identity?.IsModerator() == true;
                return true;
            }
        }
        public string AuthorId { get; set; }
        public bool IsSticky { get; set; }
        public bool IsAnnounce { get; set; }
        public bool IsUnlisted { get; set; }
        public bool IsGlobal { get; set; }

        public virtual ForumPost[] Posts
        {
            get
            {
                return new ApplicationDbContext().ForumPosts.Where(x => x.Parent == this.Id).ToArray();
            }
        }

        public virtual ForumPoll Poll { get; set; }


    }

    public class Like
    {
        public string Id { get; set; }

        [Required]
        public string User { get; set; }

        public DateTime LikedAt { get; set; }

        [Required]
        public string Topic { get; set; }
        public bool IsDislike { get; set; }
    }

    public class EditPostViewModel
    {
        public string Id { get; set; }
        [AllowHtml]
        public string Contents { get; set; }
        public string EditReason { get; set; }
    }

    public class ForumPoll
    {
        public string Id { get; set; }
        public string Description { get; set; }

        [Required]
        public virtual List<ForumPollOption> Options { get; set; }
        public bool IsActive { get; set; }
        public bool AllowMultivote { get; set; }
        public bool AllowVoteChanges { get; set; }

        [Required]
        public virtual ForumTopic Parent { get; set; }
    }

    public class ForumPollOption
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [Required]
        public virtual List<ForumPollVote> Votes { get; set; }


        [Required]
        public virtual ForumPoll Poll { get; set; }
    }

    public class ForumPollVote
    {
        public string Id { get; set; }
        
        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public virtual ForumPollOption Option { get; set; }
    }

    //Unable to generate an explicit migration because the following explicit migrations are pending: [201703161804526_more-relationship-shit]. Apply the pending explicit migrations before attempting to generate a new explicit migration.

}