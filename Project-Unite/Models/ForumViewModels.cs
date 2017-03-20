using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class CreateTopicViewModel
    {
        public string Category { get; set; }


        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }

        public bool IsAnnounce { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsSticky { get; set; }

        public bool HasPoll { get; set; }
        public string PollOptions { get; set; }
        public bool VotesChangable { get; set; }
        public bool Multivote { get; set; }
        public string Question { get; set; }
    }
}