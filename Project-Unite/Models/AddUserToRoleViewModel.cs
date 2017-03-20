using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class AddUserToRoleViewModel
    {
        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Role")]
        public string RoleId { get; set; }
    }
}