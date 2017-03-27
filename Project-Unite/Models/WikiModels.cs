using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
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

                return db.WikiPages.Where(w => w.Id != this.Id && w.Name.ToLower().Contains(this.Name.ToLower())).ToArray();
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