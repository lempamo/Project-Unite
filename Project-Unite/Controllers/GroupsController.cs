using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class GroupsController : Controller
    {
        // GET: Groups
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            return View(db.Groups);
        }

        [Authorize]
        public ActionResult ViewGroup(string id)
        {
            var db = new ApplicationDbContext();
            var group = db.Groups.FirstOrDefault(x => x.Id == id);
            if (group == null)
                return new HttpStatusCodeResult(404);

            return View(group);
        }
    }
}