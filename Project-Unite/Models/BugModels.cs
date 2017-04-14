using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class Bug
    {
        public string Id { get; set; }
        /// <summary>
        /// Just a funny name for "what category is this bug in?"
        /// </summary>
        public string Species { get; set; }
        public int Urgency { get; set; }
        public string Name { get; set; }

        public ForumPost[] Comments
        {
            get
            {
                return new ApplicationDbContext().ForumPosts.Where(x => x.Parent == this.Id).ToArray();
            }
        }

        public DateTime ReportedAt { get; set; }
        public string ReleaseId { get; set; }
        public string Reporter { get; set; }
        public bool Open { get; set; }
        public DateTime ClosedAt { get; set; }
        public string ClosedBy { get; set; }
    }

    public class BugTag
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
        public string Description { get; set; }

        public Bug[] Open
        {
            get
            {
                return new ApplicationDbContext().Bugs.Where(x => x.Species == this.Id && x.Open == true).ToArray();
            }
        }
    }

    public class PostBugViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="You must specify a name for your bug.")]
        [MaxLength(75, ErrorMessage = "Your bug's name must have at most 75 characters.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage ="Please describe your bug.")]
        [AllowHtml]
        [MinLength(20, ErrorMessage = "Your bug's description must have at least 20 characters in it.")]
        public string Description { get; set; }


        public string SpeciesId { get; set; }

        public string VersionId { get; set; }

        public string Urgency { get; set; }

        public List<SelectListItem> Urgencies
        {
            get
            {
                var items = new List<SelectListItem>();
                string[] list = new[] { "Minor", "Moderate", "Major", "Critical" };
                for (int i = 0; i < list.Length; i++)
                {
                    items.Add(new SelectListItem
                    {
                        Text = list[i],
                        Value = i.ToString()
                    });
                }
                return items;
            }
        }

        public List<SelectListItem> Species
        {
            get
            {
                var db = new ApplicationDbContext();
                var items = new List<SelectListItem>();
                foreach (var itm in db.BugTags.OrderBy(x => x.Name))
                {
                    items.Add(new SelectListItem
                    {
                        Text = itm.Name,
                        Value = itm.Id
                    });
                }
                return items;
            }
        }

        public List<SelectListItem> Versions
        {
            get
            {
                var db = new ApplicationDbContext();
                var items = new List<SelectListItem>();
                foreach(var itm in db.Downloads.OrderByDescending(x => x.PostDate))
                {
                    items.Add(new SelectListItem
                    {
                        Text = itm.Name,
                        Value = itm.Id
                    });
                }
                return items;
            }
        }
    }
}