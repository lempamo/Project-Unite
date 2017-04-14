using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}