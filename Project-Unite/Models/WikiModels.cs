using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class AddWikiPageViewModel : AddWikiCategoryViewModel
    {
        public AddWikiPageViewModel() : base()
        {
            Parents.Remove(Parents.FirstOrDefault(x => x.Value == "none"));
        }
        [AllowHtml]
        [Required(ErrorMessage = "Please enter content for your page.")]
        public string Content { get; set; }
    }

    public class AddWikiCategoryViewModel
    {
        public AddWikiCategoryViewModel()
        {
            var db = new ApplicationDbContext();
            Parents = new List<SelectListItem>();
            Parents.Add(new SelectListItem
            {
                Value = "none",
                Text = "No parent"
            });
            foreach(var cat in db.WikiCategories)
            {
                Parents.Add(new SelectListItem
                {
                    Value = cat.Id,
                    Text = cat.Name
                });
            }
            db.Dispose();
        }

        public List<SelectListItem> Parents { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage ="Please name your category/page.")]
        [MinLength(5, ErrorMessage ="Your category/page's name must be at least 5 characters long.")]
        [MaxLength(25, ErrorMessage ="Your category/page's name must be at most 25 characters long.")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a parent category.")]
        public string ParentId { get; set; }
    }

    public class WikiCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        
        public WikiCategory[] Children
        {
            get
            {
                var db = new ApplicationDbContext();

                return db.WikiCategories.Where(x => x.Parent == this.Id).ToArray();
            }
        }

        public WikiPage[] Pages
        {
            get
            {
                var db = new ApplicationDbContext();

                return db.WikiPages.Where(w => w.CategoryId == this.Id).ToArray();
            }
        }
    }

    public class WikiViewModel
    {
        public IEnumerable<WikiCategory> Categories { get; set; }
        public WikiPage Page { get; set; }
    }

    public class WikiPage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        
        //I stole this feature from wikipedia lol. I like the idea of disambiguation of multiple pages with the same name.
        public WikiPage[] AmbiguousReferences
        {
            get
            {
                var db = new ApplicationDbContext();

                var ambiguous1 = db.WikiPages.Where(w => w.Id != this.Id && w.Name.ToLower().Contains(this.Name.ToLower())).ToArray();
                var ambiguous2 = db.WikiPages.Where(w => w.Id != this.Id && this.Name.ToLower().Contains(w.Name.ToLower())).ToArray();

                var list = new List<WikiPage>();
                list.AddRange(ambiguous1);
                list.AddRange(ambiguous2);

                return list.ToArray();
            }


            
        }

        public Like[] Likes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(x => x.Topic == this.Id && x.IsDislike == false).ToArray();
            }
        }

        public Like[] Dislikes
        {
            get
            {
                return new ApplicationDbContext().Likes.Where(x => x.Topic == this.Id && x.IsDislike == true).ToArray();
            }
        }

        public string Contents { get; set; }

        public ForumPostEdit[] EditHistory
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.ForumPostEdits.Where(x => x.Parent == this.Id).ToArray();
            }
        }
    }
}