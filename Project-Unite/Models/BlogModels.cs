using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class PostBlogViewModel
    {
        [Required(ErrorMessage="Please enter a name for your post!")]
        [MinLength(5, ErrorMessage ="Your post's name must have at least 5 characters.")]
        [MaxLength(50, ErrorMessage = "Your post's name must have at least 50 characters.")]
        public string Name { get; set; }

        [AllowHtml]
        [Required(ErrorMessage ="You can't post an empty blog post!")]
        [MinLength(20, ErrorMessage = "Your post must have at least 20 characters.")]
        public string Contents { get; set; }
    }

    public class BlogPost
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public ForumPost[] Comments
        {
            get
            {
                return new ApplicationDbContext().ForumPosts.Where(x => x.Parent == this.Id).ToArray();
            }
        }
        public Like[] Likes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(x => x.Topic == this.Id&&x.IsDislike == false).ToArray();
            }
        }

        public string Name { get; set; }

        public string Summary
        {
            get
            {
                return Contents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }

        public Like[] Dislikes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(x => x.Topic == this.Id && x.IsDislike == true).ToArray();

            }
        }

        public DateTime PostedAt { get; set; }

        public string Contents { get; set; }
    }
}