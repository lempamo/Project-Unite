using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [Authorize]
    public class ContestsController : Controller
    {
        [AllowAnonymous]
        // GET: Contests
        public ActionResult Index()
        {
            var model = new ApplicationDbContext().Contests.ToArray();
            return View(model);
        }

        public ActionResult ViewContest(string id)
        {
            var db = new ApplicationDbContext();
            var c = db.Contests.FirstOrDefault(x => x.Id == id);
            if (c == null)
                return new HttpStatusCodeResult(404);
            return View(c);
        }

        public ActionResult ViewSubmission(string id)
        {
            var db = new ApplicationDbContext();
            var e = db.ContestEntries.FirstOrDefault(x => x.Id == id);
            if (e == null)
                return new HttpStatusCodeResult(404);
            return View(e);
        }

        [RequiresAdmin]
        public ActionResult CloseContest(string id)
        {
            var db = new ApplicationDbContext();
            var c = db.Contests.FirstOrDefault(x => x.Id == id);
            if (c == null)
                return new HttpStatusCodeResult(404);
            c.EndsAt = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult SubmitEntry(string id)
        {
            var db = new ApplicationDbContext();
            var contest = db.Contests.FirstOrDefault(x => x.Id == id);
            if (contest == null)
                return new HttpStatusCodeResult(404);
            if (contest.UserSubmitted(User.Identity.GetUserId()))
                return new HttpStatusCodeResult(403);

            var model = new SubmitContestEntryViewModel();
            model.ContestId = contest.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitEntry(SubmitContestEntryViewModel model)
        {
            if(model.Download != null)
            {
                if (!model.Download.FileName.ToLower().EndsWith(".zip"))
                    ModelState.AddModelError("Download", new Exception("The uploaded file is not a .zip file."));
            }

            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();
            var entry = new ContestEntry();
            entry.Name = model.Name;
            entry.Description = model.Description;
            entry.PostedAt = DateTime.Now;
            entry.Disqualified = false;
            entry.AuthorId = User.Identity.GetUserId();
            entry.ContestId = model.ContestId;
            entry.VideoId = model.VideoID;
            string allowed = "abcdefghijklmnopqrstuvwxyz1234567890_";
            entry.Id = entry.Name.ToLower();
            foreach (var ch in entry.Id.ToCharArray())
                if (!allowed.Contains(ch))
                    entry.Id = entry.Id.Replace(ch, '_');
            if (model.Download != null)
            {
                string fname = model.Download.FileName.ToLower().Replace(".zip", "");
                foreach (var ch in fname.ToCharArray())
                    if (!allowed.Contains(ch))
                        fname = fname.Replace(ch, '_');
                fname += ".zip";
                string serverpath = "~/Uploads/" + ACL.UserNameRaw(User.Identity.GetUserId()) + "/ContestSubmissions/" + model.ContestId;
                string mapped = Server.MapPath(serverpath);
                string mappedwithfilename = Path.Combine(mapped, fname);
                string dbpath = serverpath.Remove(0, 1) + "/" + fname;
                if (!Directory.Exists(mapped))
                    Directory.CreateDirectory(mapped);
                model.Download.SaveAs(mappedwithfilename);
                entry.DownloadURL = dbpath;
            }

            db.ContestEntries.Add(entry);
            db.SaveChanges();

            foreach(var user in db.Users.ToArray())
            {
                if (user.HighestRole.IsAdmin)
                {
                    NotificationDaemon.NotifyUser(entry.AuthorId, user.Id, "New contest entry!", "A new entry has been submitted to the \"" + db.Contests.FirstOrDefault(x=>x.Id==entry.ContestId).Name + "\" contest.", Url.Action("ViewSubmission", "Contests", new { id = entry.Id }));
                }
            }

            return RedirectToAction("ViewSubmission", "Contests", new { id = entry.Id });
        }

        [RequiresAdmin]
        public ActionResult CreateContest()
        {
            var model = new CreateContestViewModel();
            model.EndDate = DateTime.Now;
            return View(model);
        }

        [RequiresAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContest(CreateContestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();

            string allowed = "abcdefghijklmnopqrstuvwxyz_0123456789";

            var c = new Contest();

            c.Name = model.Name;
            c.Description = model.Description;
            c.StartedAt = DateTime.Now;
            c.EndsAt = model.EndDate;
            c.VideoId = model.VideoId;
            string id = c.Name.ToLower() + "_" + db.Contests.Count();
            foreach (char ch in id.ToCharArray())
                if (!allowed.Contains(ch))
                    id = id.Replace(ch, '_');
            c.Id = id;
            c.CodepointReward1st = model.GoldReward;
            c.CodepointReward2nd = model.SilverReward;
            c.CodepointReward3rd = model.BronzeReward;
            db.Contests.Add(c);
            db.SaveChanges();

            NotificationDaemon.NotifyEveryone(User.Identity.GetUserId(), "A contest has just started.", "A new contest has been open! Contest: " + c.Name, Url.Action("ViewContest", new { id = c.Id }));

            return RedirectToAction("ViewContest", new { id = c.Id });
        }
    }
}