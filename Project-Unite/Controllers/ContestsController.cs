using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        [RequiresAdmin]
        public ActionResult CreateContest()
        {
            var model = new CreateContestViewModel();
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

            return RedirectToAction("ViewContest", new { id = c.Id });
        }
    }
}