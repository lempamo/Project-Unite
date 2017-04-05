using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [Authorize]
    public class DeveloperController : Controller
    {
        // GET: Developer
        public ActionResult Index()
        {
            ViewBag.Developer = true;
            return View();
        }

        public ActionResult Releases()
        {
            var db = new ApplicationDbContext();
            return View(db.Downloads);
        }
    }
}