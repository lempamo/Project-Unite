﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class CreateContestViewModel
    {
        [Required(AllowEmptyStrings =false, ErrorMessage ="You must name your contest!")]
        [MinLength(5, ErrorMessage ="Your contest name must contain at least 5 characters.")]
        [MaxLength(35, ErrorMessage ="Your contest's name must not have more than 35 characters!")]
        public string Name { get; set; }


        [AllowHtml]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please describe your contest!")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Please set an end date for the contest.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage ="Please specify a Codepoint reward for the gold winner.")]
        public long GoldReward { get; set; }

        [Required(ErrorMessage = "Please specify a Codepoint reward for the silver winner.")]
        public long SilverReward { get; set; }

        [Required(ErrorMessage = "Please specify a Codepoint reward for the bronze winner.")]
        public long BronzeReward { get; set; }


        public string VideoId { get; set; }
    }

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

        public bool UserSubmitted(string uid)
        {
            return Entries.FirstOrDefault(x => x.AuthorId == uid) != null;
        }

        public bool IsEnded { get; set; }

        public ContestEntry[] Entries
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.ContestEntries.Where(x => x.ContestId == this.Id).ToArray();
            }
        }

    }

    public class DisqualifySubmissionViewModel
    {
        public string Entry { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="You must provide a reason.")]
        public string Reason { get; set; }
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
        public string DisqualifiedBy { get; set; }
        public string DisqualifiedReason { get; set; }

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

    public class SubmitContestEntryViewModel
    {
        public string ContestId { get; set; }

        public List<SelectListItem> Contests
        {
            get
            {
                var db = new ApplicationDbContext();
                var list = new List<SelectListItem>();
                foreach (var c in db.Contests.Where(x => x.IsEnded == false).OrderByDescending(x => x.StartedAt).ToArray())
                {
                    list.Add(new SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    });
                }
                return list;
            }
        }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Please name your submission!")]
        [MaxLength(55, ErrorMessage ="Your submission's name must have less than 55 characters in it.")]
        [MinLength(5, ErrorMessage ="Your submission's name must be at least 5 characters long.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Please describe your submission!")]
        [AllowHtml]
        public string Description { get; set; }

        public HttpPostedFileBase Download { get; set; }

        public string VideoID { get; set; }
    }
}