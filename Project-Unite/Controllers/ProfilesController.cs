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

        public ActionResult PostContent(UserPost model)
        {
            var db = new ApplicationDbContext();
            model.Id = Guid.NewGuid().ToString();
            model.PostedAt = DateTime.Now;
            model.UserId = User.Identity.GetUserId();
            db.UserPosts.Add(model);
            db.SaveChanges();
            return RedirectToAction("ViewProfile", "Profiles", new { id = ACL.UserNameRaw(User.Identity.GetUserId()) });
            
        }
    }
}