using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        // GET: Profiles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewProfile(string id)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.DisplayName == id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            return View(user);
        }
    }
}