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

        public ActionResult Upvote(string id)
        {
            string uid = User.Identity.GetUserId();
            var db = new ApplicationDbContext();
            var s = db.ContestEntries.FirstOrDefault(x => x.Id == id);
            if (s == null)
                return new HttpStatusCodeResult(404);
            var like = db.Likes.FirstOrDefault(x => x.User == uid && x.Topic == id);
            if (like != null)
            {
                if (like.IsDislike == false)
                    return new HttpStatusCodeResult(403);
                else
                    like.IsDislike = false;
            }
            else
            {
                like = new Like();
                like.Id = Guid.NewGuid().ToString();
                like.IsDislike = false;
                like.User = uid;
                like.Topic = id;
                db.Likes.Add(like);
            }
            db.SaveChanges();
            return RedirectToAction("ViewSubmission", new { id = id });

        }

        public ActionResult Downvote(string id)
        {
            string uid = User.Identity.GetUserId();
            var db = new ApplicationDbContext();
            var s = db.ContestEntries.FirstOrDefault(x => x.Id == id);
            if (s == null)
                return new HttpStatusCodeResult(404);
            var like = db.Likes.FirstOrDefault(x => x.User == uid && x.Topic == id);
            if (like != null)
            {
                if (like.IsDislike == true)
                    return new HttpStatusCodeResult(403);
                else
                    like.IsDislike = true;
            }
            else
            {
                like = new Like();
                like.Id = Guid.NewGuid().ToString();
                like.IsDislike = true;
                like.User = uid;
                like.Topic = id;
                db.Likes.Add(like);
            }
            db.SaveChanges();
            return RedirectToAction("ViewSubmission", new { id = id });

        }

        [RequiresAdmin]
        public ActionResult Disqualify(string id)
        {
            var db = new ApplicationDbContext();
            var c = db.ContestEntries.FirstOrDefault(x => x.Id == id);
            if (c == null)
                return new HttpStatusCodeResult(404);
            var model = new DisqualifySubmissionViewModel();
            model.Entry = id;
            return View(model);
        }

        [RequiresAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disqualify(DisqualifySubmissionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var db = new ApplicationDbContext();
            var e = db.ContestEntries.FirstOrDefault(x => x.Id == model.Entry);
            if (e == null)
                return new HttpStatusCodeResult(404);
            e.Disqualified = true;
            e.DisqualifiedBy = User.Identity.GetUserId();
            e.DisqualifiedReason = model.Reason;
            db.SaveChanges();

            NotificationDaemon.NotifyUser(User.Identity.GetUserId(), e.AuthorId, "Submission disqualified.", $@"We have disqualified your contest submission ""{e.Name}"".

<strong>Reason:</strong> {e.DisqualifiedReason}", Url.Action("ViewSubmission", new { id = model.Entry }));

            return RedirectToAction("ViewSubmission", new { id = model.Entry });
        }

        public ActionResult RemoveVote(string id)
        {
            string uid = User.Identity.GetUserId();
            var db = new ApplicationDbContext();
            var s = db.ContestEntries.FirstOrDefault(x => x.Id == id);
            if (s == null)
                return new HttpStatusCodeResult(404);
            var like = db.Likes.FirstOrDefault(x => x.User == uid && x.Topic == id);
            if (like != null)
            {
                db.Likes.Remove(like);
            }
            else
            {
                return new HttpStatusCodeResult(403);
            }
            db.SaveChanges();
            return RedirectToAction("ViewSubmission", new { id = id });

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
            entry.Id = entry.Name.ToLower() + "_" + db.ContestEntries.Count().ToString();
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