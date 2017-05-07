using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class GroupsController : Controller
    {
        // GET: Groups
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            return View(db.Groups);
        }

        [Authorize]
        public ActionResult JoinGroup(string id)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId());
            var group = db.Groups.FirstOrDefault(x => x.Id == id);
            if (group == null)
                return new HttpStatusCodeResult(404);
            user.GroupId = id;
            db.SaveChanges();
            return RedirectToAction("ViewGroup", "Groups", new { id = id });
        }


        [Authorize]
        public ActionResult ViewGroup(string id)
        {
            var db = new ApplicationDbContext();
            var group = db.Groups.FirstOrDefault(x => x.Id == id);
            if (group == null)
                return new HttpStatusCodeResult(404);

            return View(group);
        }
    }
}