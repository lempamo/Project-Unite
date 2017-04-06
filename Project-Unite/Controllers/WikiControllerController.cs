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
        public ActionResult Index(string id = "")
        {
            var db = new ApplicationDbContext();
            var model = new WikiViewModel();
            var wikicategories = db.WikiCategories.Where(x => x.Parent == "none");
            model.Categories = wikicategories;
            model.Page = db.WikiPages.FirstOrDefault(x => x.Id == id);
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
    }
}