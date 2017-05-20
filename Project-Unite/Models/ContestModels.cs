using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class Contest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string VideoId { get; set; }
        public long CodepointReward1st { get; set; }
        public long CodepointReward2nd { get; set; }
        public long CodepointReward3rd { get; set; }


        public bool IsEnded
        {
            get
            {
                return DateTime.Now >= EndsAt;
            }
        }

        public ContestEntry[] Entries
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.ContestEntries.Where(x => x.ContestId == this.Id).ToArray();
            }
        }

    }

    public class ContestEntry
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string ContestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoId { get; set; }
        public string DownloadURL { get; set; }
        public DateTime PostedAt { get; set; }
        public bool Disqualified { get; set; }

        public Like[] Downvotes
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.Likes.Where(x => x.Topic == this.Id && x.IsDislike).ToArray();
            }
        }

        public Like[] Upvotes
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.Likes.Where(x => x.Topic == this.Id && !x.IsDislike).ToArray();
            }
        }
    }
}