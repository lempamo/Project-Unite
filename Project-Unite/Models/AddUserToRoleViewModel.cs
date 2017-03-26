using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class AddUserToRoleViewModel
    {
        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Role")]
        public string RoleId { get; set; }

        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Users { get; set; }
    }
}