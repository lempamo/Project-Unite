using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class SimpleSearch
    {
        public string Query { get; set; }
    }

    public class SearchResult
    {
        public IEnumerable<ForumPost> ForumPosts { get; set; }
        public IEnumerable<ForumTopic> ForumTopics { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Skin> Skins { get; set; }
        public IEnumerable<Download> Downloads { get; set; }
        public IEnumerable<WikiPage> WikiPages { get; set; }
    }
}