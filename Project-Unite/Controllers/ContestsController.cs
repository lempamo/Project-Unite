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
    }
}