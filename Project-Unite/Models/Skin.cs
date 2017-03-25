using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class Skin
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string VersionId { get; set; }
        public string DownloadUrl { get; set; }
        public string ScreenshotUrl { get; set; }
        public DateTime PostedAt { get; set; }

        public Like[] Likes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(x => x.Topic == this.Id).Where(x => x.IsDislike == false).ToArray();
            }
        }

        public Like[] Dislikes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(x => x.Topic == this.Id).Where(x => x.IsDislike == true).ToArray();
            }
        }

        public View[] Views
        {
            get
            {
                return new ApplicationDbContext().Views.Where(x => x.ContentId == this.Id).ToArray();
            }
        }
    }

    public class View
    {
        public string Id { get; set; }
        public string ContentId { get; set; }
        public string UserId { get; set; }
    }

    public class CreateSkinViewModel
    {
        [Required]
        [MaxLength(128, ErrorMessage = "Your title may not contain more than 128 characters.")]
        [MinLength(5, ErrorMessage = "You need to supply a valuable title.")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage ="Your short description may not contain more than 500 characters.")]
        public string ShortDescription { get; set; }

        [Required]
        [DataType(DataType.Html)]
        public string LongDescription { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase SkinFile { get; set; }
    }
}