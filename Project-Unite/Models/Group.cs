using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class GroupViewModel
    {
        public List<SelectListItem> Publicities
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Value="public", Text="Public"},
                    new SelectListItem { Value="publici", Text="Public (Invite Only)"},
                    new SelectListItem { Value="private", Text="Private"},
                    new SelectListItem { Value="privatei", Text="Private (Invite Only)" }
                };
            }
        }

        [Required]
        [MaxLength(25, ErrorMessage = "Your group's name must have a maximum of 25 characters in it.")]
        [MinLength(5, ErrorMessage = "You must set a name with at least 5 characters in it.")]
        public string Name { get; set; }

        [Required]
        public string Publicity { get; set; }

        [Required]
        [MaxLength(6, ErrorMessage = "Hexadecimal color values can only have 6 or less digits.")]
        [MinLength(3, ErrorMessage = "Hexadecimal color values must have at least 3 digits.")]
        public string BannerColorHex { get; set; }

        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [MaxLength(4, ErrorMessage = "Your Short Name can only have 4 characters. Think of it like an acronym.")]
        public string ShortName { get; set; }

    }


    public class Group
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage ="Your group's name must have a maximum of 25 characters in it.")]
        [MinLength(5, ErrorMessage ="You must set a name with at least 5 characters in it.")]
        public string Name { get; set; }

        [Required]
        public int Publicity { get; set; }

        [Required]
        [MaxLength(6, ErrorMessage ="Hexadecimal color values can only have 6 or less digits.")]
        [MinLength(3, ErrorMessage ="Hexadecimal color values must have at least 3 digits.")]
        public string BannerColorHex { get; set; }

        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [MaxLength(4, ErrorMessage ="Your Short Name can only have 4 characters. Think of it like an acronym.")]
        public string ShortName { get; set; }

        [Required]
        public double RawReputation { get; set; }

        public ApplicationUser[] Users
        {
            get
            {
                var db = new ApplicationDbContext();
                return db.Users.Where(x => x.GroupId == this.Id).ToArray();
            }
        }

        public int Reputation
        {
            get
            {
                return ((int)Math.Round(RawReputation));
            }
        }

    }
}