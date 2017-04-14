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

        [Authorize]
        public ActionResult OpenBug(string id)
        {
            var db = new ApplicationDbContext();
            var bug = db.Bugs.FirstOrDefault(x => x.Id == id);
            if (bug == null)
                return new HttpStatusCodeResult(404);
            bug.Open = true;
            db.SaveChanges();
            return RedirectToAction("ViewCategory", new { id = bug.Species });
        }

        [Authorize]
        public ActionResult CloseBug(string id)
        {
            var db = new ApplicationDbContext();
            var bug = db.Bugs.FirstOrDefault(x => x.Id == id);
            if (bug == null)
                return new HttpStatusCodeResult(404);
            bug.Open = false;
            bug.ClosedAt = DateTime.Now;
            bug.ClosedBy = User.Identity.GetUserId();
            db.SaveChanges();
            return RedirectToAction("ViewCategory", new { id = bug.Species });
        }

        [Authorize]
        public ActionResult PostBug()
        {
            var model = new PostBugViewModel();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostBug(PostBugViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var db = new ApplicationDbContext();
            var bug = new Bug();
            bug.Name = model.Name;

            string allowed = "abcdefghijklmnopqrstuvwxyz1234567890_-";

            string id = bug.Name.ToLower() + "_" + db.Bugs.Count().ToString();

            foreach(var c in id.ToCharArray())
            {
                if (!allowed.Contains(c))
                    id = id.Replace(c, '_');
            }

            bug.Id = id;
            bug.Open = true;
            bug.ClosedAt = DateTime.Now;
            bug.Reporter = User.Identity.GetUserId();
            bug.ReportedAt = DateTime.Now;
            bug.Species = model.SpeciesId;
            bug.Urgency = int.Parse(model.Urgency);
            bug.ReleaseId = model.VersionId;
            var comment = new ForumPost();
            comment.AuthorId = User.Identity.GetUserId();
            comment.Body = model.Description;
            comment.Id = Guid.NewGuid().ToString();
            comment.Parent = bug.Id;
            comment.PostedAt = DateTime.Now;
            db.ForumPosts.Add(comment);
            db.Bugs.Add(bug);
            db.SaveChanges();

            return RedirectToAction("ViewBug", new { id = bug.Id });
        }
    }
}