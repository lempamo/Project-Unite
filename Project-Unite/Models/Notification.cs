using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Discriminator { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string AvatarUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActionUrl { get; set; }
    }
}