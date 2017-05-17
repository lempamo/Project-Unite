using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [RequiresModerator]
    [Authorize]
    public class ModeratorController : Controller
    {
        // GET: Moderator
        public ActionResult Index()
        {
            ViewBag.Moderator = true;
            return View();
        }

        public ActionResult UserDetails(string id)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.DisplayName == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);
            return View(usr);
        }

        public ActionResult Users()
        {
            return View(new ApplicationDbContext().Users);
        }

        public ActionResult Unban(string id, string returnUrl = "")
        {
            var db = new ApplicationDbContext();

            var usr = db.Users.FirstOrDefault(x => x.Id == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);
            if (usr.IsBanned == false) //we don't need to re-unban the user... save the SQL queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            usr.IsBanned = false;

            db.AuditLogs.Add(new Models.AuditLog(User.Identity.GetUserId(), AuditLogLevel.Moderator, $@"Moderator has unbanned {ACL.UserNameRaw(id)}."));

            db.SaveChanges();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Users");
            else
                return Redirect(returnUrl);
        }


        public ActionResult Ban(string id, string returnUrl = "")
        {
            var db = new ApplicationDbContext();

            var usr = db.Users.FirstOrDefault(x => x.Id == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);
            if (usr.IsBanned == true) //we don't need to re-ban the user... save the SQL queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            usr.IsBanned = true;
            usr.BannedAt = DateTime.Now;
            usr.BannedBy = User.Identity.GetUserId();

            db.AuditLogs.Add(new Models.AuditLog(User.Identity.GetUserId(), AuditLogLevel.Moderator, $@"Moderator has banned {ACL.UserNameRaw(id)}."));

            db.SaveChanges();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Users");
            else
                return Redirect(returnUrl);
        }


        public ActionResult Unmute(string id, string returnUrl = "")
        {
            var db = new ApplicationDbContext();

            var usr = db.Users.FirstOrDefault(x => x.Id == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);
            if (usr.IsMuted == false) //we don't need to re-unmute the user... save the SQL queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            usr.IsMuted = false;

            db.AuditLogs.Add(new Models.AuditLog(User.Identity.GetUserId(), AuditLogLevel.Moderator, $@"Moderator has un-muted {ACL.UserNameRaw(id)}."));

            db.SaveChanges();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Users");
            else
                return Redirect(returnUrl);
        }

        public ActionResult GetUsername(string id)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.Id == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);
            return Content(usr.DisplayName);
        }

        public ActionResult ChangeUserName(string id, string newName)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.Id == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);

            usr.DisplayName = newName;

            db.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        public ActionResult LockTopic(string id)
        {
            var db = new ApplicationDbContext();
            var forum = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (forum == null)
                return new HttpStatusCodeResult(404);
            string perm = "CanLockTopics";
            var uid = User.Identity.GetUserId();

            if (forum.IsLocked == true) //Save the DB queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            forum.IsLocked = true;

            db.AuditLogs.Add(new AuditLog(uid, AuditLogLevel.Moderator, $"User has locked topic \"{forum.Subject}\" by {ACL.UserNameRaw(forum.AuthorId)}."));
            db.SaveChanges();

            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }

        public ActionResult UnlockTopic(string id)
        {
            var db = new ApplicationDbContext();
            var forum = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (forum == null)
                return new HttpStatusCodeResult(404);
            string perm = "CanUnlockTopics";
            var uid = User.Identity.GetUserId();

            if (forum.IsLocked == false) //Save the DB queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            forum.IsLocked = false;

            db.AuditLogs.Add(new AuditLog(uid, AuditLogLevel.Moderator, $"User has unlocked topic \"{forum.Subject}\" by {ACL.UserNameRaw(forum.AuthorId)}."));

            db.SaveChanges();

            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }

        public ActionResult List(string id)
        {
            var db = new ApplicationDbContext();
            var forum = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (forum == null)
                return new HttpStatusCodeResult(404);
            string perm = "CanUnlistTopics";
            var uid = User.Identity.GetUserId();

            if (forum.IsUnlisted == false) //Save the DB queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            forum.IsUnlisted = false;


            db.SaveChanges();
            db.AuditLogs.Add(new AuditLog(uid, AuditLogLevel.Moderator, $"User has listed topic \"{forum.Subject}\" by {ACL.UserNameRaw(forum.AuthorId)}."));
            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }

        public ActionResult Unlist(string id)
        {
            var db = new ApplicationDbContext();
            var forum = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (forum == null)
                return new HttpStatusCodeResult(404);
            string perm = "CanUnlistTopics";
            var uid = User.Identity.GetUserId();

            if (forum.IsUnlisted == true) //Save the DB queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            forum.IsUnlisted = true;

            db.AuditLogs.Add(new AuditLog(uid, AuditLogLevel.Moderator, $"User has unlisted topic \"{forum.Subject}\" by {ACL.UserNameRaw(forum.AuthorId)}."));
            db.SaveChanges();

            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }


        public ActionResult Bans()
        {
            var model = new ModeratorBanListViewModel();
            var db = new ApplicationDbContext();

            model.UserBans = db.Users.Where(x => x.IsBanned == true);
            model.IPBans = db.BannedIPs;

            return View(model);
        }

        public ActionResult Logs()
        {
            var db = new ApplicationDbContext();

            return View(db.AuditLogs.Where(x => x.Level != AuditLogLevel.Admin));
        }

        public ActionResult Mute(string id, string returnUrl = "")
        {
            var db = new ApplicationDbContext();

            var usr = db.Users.FirstOrDefault(x => x.Id == id);
            if (usr == null)
                return new HttpStatusCodeResult(404);
            if (usr.IsMuted == true) //we don't need to re-mute the user... save the SQL queries...
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            usr.IsMuted = true;
            usr.MutedAt = DateTime.Now;
            usr.MutedBy = User.Identity.GetUserId();

            db.AuditLogs.Add(new Models.AuditLog(User.Identity.GetUserId(), AuditLogLevel.Moderator, $@"Moderator has muted {ACL.UserNameRaw(id)}."));

            db.SaveChanges();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Users");
            else
                return Redirect(returnUrl);
        }

        public ActionResult AnnounceTopic(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (topic == null)
                return new HttpStatusCodeResult(404);
            topic.IsAnnounce = !topic.IsAnnounce;
            db.SaveChanges();
            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }

        public ActionResult GlobalTopic(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (topic == null)
                return new HttpStatusCodeResult(404);
            topic.IsGlobal = !topic.IsGlobal;
            db.SaveChanges();
            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }

        public ActionResult StickyTopic(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (topic == null)
                return new HttpStatusCodeResult(404);
            topic.IsSticky = !topic.IsSticky;
            db.SaveChanges();
            return RedirectToAction("ViewTopic", "Forum", new { id = id });
        }

    }
}