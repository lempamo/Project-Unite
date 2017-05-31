using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [Authorize]
    public class QuotesController : Controller
    {
        // GET: Quotes
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.Quote model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var db = new Models.ApplicationDbContext();
            model.Id = (db.Quotes.Count() + 1).ToString();
            model.IsApproved = false;
            db.Quotes.Add(model);
            db.SaveChanges();

            var users = db.Users.ToArray();
            foreach (var user in users)
            {
                try
                {
                    if (user.HighestRole.IsAdmin)
                    {
                        NotificationDaemon.NotifyUser(User.Identity.GetUserId(), user.Id, "New quote submitted.", "Please review user-submitted quotes.", Url.Action("ReviewAll"));
                    }
                }
                catch { }
            }
            return View(model);
        }

        [RequiresModerator]
        public ActionResult ReviewAll()
        {
            var db = new ApplicationDbContext();
            return View(db.Quotes.Where(x => x.IsApproved == false));
        }

        [RequiresModerator]
        public ActionResult Deny(string id)
        {
            var db = new ApplicationDbContext();
            var quote = db.Quotes.FirstOrDefault(x => x.Id == id);
            if (quote == null)
                return new HttpStatusCodeResult(404);
            if (quote.IsApproved == true)
                return new HttpStatusCodeResult(403);
            db.Quotes.Remove(quote);
            db.SaveChanges();
            return RedirectToAction("ReviewAll");

        }

        [RequiresModerator]
        public ActionResult Approve(string id)
        {
            var db = new ApplicationDbContext();
            var quote = db.Quotes.FirstOrDefault(x => x.Id == id);
            if (quote == null)
                return new HttpStatusCodeResult(404);
            quote.IsApproved = true;
            db.SaveChanges();
            return RedirectToAction("ReviewAll");
        }
    }
}