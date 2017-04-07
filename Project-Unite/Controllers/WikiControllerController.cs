using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class WikiController : Controller
    {
        public ActionResult Index(string id = "", bool triedtolikeowntopic=false)
        {
            var db = new ApplicationDbContext();
            var model = new WikiViewModel();
            var wikicategories = db.WikiCategories.Where(x => x.Parent == "none");
            model.Categories = wikicategories;
            model.Page = db.WikiPages.FirstOrDefault(x => x.Id == id);
            if (triedtolikeowntopic)
                ViewBag.Error = "You cannot like or dislike your own wiki page.";
            return View(model);
        }

        public ActionResult Random()
        {
            var db = new ApplicationDbContext();
            var rnd = new Random();
            var index = rnd.Next(0, db.WikiPages.Count());
            if (db.WikiPages.Count() == 0)
                return RedirectToAction("Index");

            var wiki = db.WikiPages.ToArray()[index].Id;
            return RedirectToAction("Index", new { id = wiki });
        }

        [Authorize]
        public ActionResult AddPage()
        {
            return View(new AddWikiPageViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPage(AddWikiPageViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();
            var page = new WikiPage();
            page.CategoryId = model.ParentId;
            page.Contents = model.Content;
            page.Name = model.Name;

            string allowed = "abcdefghijklmnopqrstuvwxyz1234567890_";
            page.Id = page.Name.ToLower();
            foreach(var c in page.Id.ToCharArray())
            {
                if (!allowed.Contains(c))
                    page.Id = page.Id.Replace(c, '_');
            }

            var edit = new ForumPostEdit();
            edit.Id = Guid.NewGuid().ToString();
            edit.Parent = page.Id;
            edit.EditedAt = DateTime.Now;
            edit.EditReason = "Page created.";
            edit.PreviousState = page.Contents;
            edit.UserId = User.Identity.GetUserId();

            db.ForumPostEdits.Add(edit);
            db.WikiPages.Add(page);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = edit.Id });
        }

        [Authorize]
        public ActionResult DislikePage(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.WikiPages.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.EditHistory.OrderBy(x => x.EditedAt).First().UserId == User.Identity.GetUserId())
                return RedirectToAction("Index", new { id = id, triedtolikeowntopic = true });
            var like = db.Likes.Where(x => x.Topic == topic.Id).FirstOrDefault(x => x.User == uid);
            if (like != null)
            {
                if (like.IsDislike == false)
                {
                    like.IsDislike = true;
                }
                else
                {
                    db.Likes.Remove(like);
                }
            }
            else
            {
                like = new Models.Like();
                like.Id = Guid.NewGuid().ToString();
                like.User = User.Identity.GetUserId();
                like.Topic = topic.Id;
                like.LikedAt = DateTime.Now;
                like.IsDislike = true;
                db.Likes.Add(like);
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        [Authorize]
        public ActionResult LikePage(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.WikiPages.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.EditHistory.OrderBy(x=>x.EditedAt).First().UserId == User.Identity.GetUserId())
                return RedirectToAction("Index", new { id = id, triedtolikeowntopic = true });
            var like = db.Likes.Where(x => x.Topic == topic.Id).FirstOrDefault(x => x.User == uid);
            if (like != null)
            {
                if (like.IsDislike == true)
                {
                    like.IsDislike = false;
                }
                else
                {
                    db.Likes.Remove(like);
                }
            }
            else
            {
                like = new Models.Like();
                like.Id = Guid.NewGuid().ToString();
                like.User = User.Identity.GetUserId();
                like.Topic = topic.Id;
                like.LikedAt = DateTime.Now;
                like.IsDislike = false;
                db.Likes.Add(like);
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

    }
}