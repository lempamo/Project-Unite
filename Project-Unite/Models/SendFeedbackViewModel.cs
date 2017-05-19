using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class SendFeedbackViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="You must provide a name so we can address you properly.")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "You must provide a valid email address so we can reply to your feedback.")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a subject.")]
        [MinLength(5, ErrorMessage ="Your subject must be at least 5 characters long.")]
        [MaxLength(35, ErrorMessage = "Your subject must be less than 35 characters long.")]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Please enter a body for your feedback email.")]
        [AllowHtml]
        public string Body { get; set; }

        public string FeedbackType { get; set; }
        
        public List<SelectListItem> FeedbackTypes
        {
            get
            {
                string[] types = new string[]
                {
                    "Feature Request - Website",
                    "Feature Request - ShiftOS Client",
                    "Feature Request - API",
                    "Security and Privacy",
                    "Discord",
                    "YouTube Channel",
                    "Ban Appeals",
                    "Other"
                };
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var type in types)
                    items.Add(new SelectListItem
                    {
                        Value = type,
                        Text = type
                    });
                return items;
            }
        }
    }
}