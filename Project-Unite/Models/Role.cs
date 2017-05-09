using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project_Unite.Models
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public string ColorHex { get; set; }

        public int Priority { get; set; }

        public bool IsModerator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
    }
}