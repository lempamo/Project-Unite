using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class BugsController : Controller
    {
        // GET: Bugs
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            return View(db.BugTags);
        }

        public ActionResult ViewCategory(string id)
        {
            var db = new ApplicationDbContext();
            var cat = db.BugTags.FirstOrDefault(x => x.Id == id);
            if (cat == null)
                return new HttpStatusCodeResult(404);
            return View(cat);
        }

        public ActionResult ViewBug(string id)
        {
            var db = new ApplicationDbContext();
            var bug = db.Bugs.FirstOrDefault(x => x.Id == id);
            if (bug == null)
                return new HttpStatusCodeResult(404);
            var model = new ViewBugViewModel();
            model.BugData = bug;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ViewBug(ViewBugViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var db = new ApplicationDbContext();
            var post = new ForumPost();
            post.Id = Guid.NewGuid().ToString();
            post.AuthorId = User.Identity.GetUserId();
            post.Body = model.Comment;
            post.Parent = model.BugData.Id;
            post.PostedAt = DateTime.Now;
            db.ForumPosts.Add(post);
            db.SaveChanges();
            model.Comment = "";
            return View(model);
        }
    }
}