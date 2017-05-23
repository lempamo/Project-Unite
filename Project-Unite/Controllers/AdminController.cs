using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    //We have a custom ACL implementation so we do not need to use the ASP.NET role system to check if a user has an ACL rule.
    [Authorize]
    [RequiresAdmin]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string id = "home")
        {
            ViewBag.Page = id;
            return View();
        }

        public ActionResult RoleDetails(string id)
        {
            var db = new ApplicationDbContext();
            Role role = null;
            foreach (var r in db.Roles.ToArray())
            {
                if (r is Role)
                    if ((r as Role).Id == id)
                        role = r as Role;
            }
            return View(role);
        }
    }
}
