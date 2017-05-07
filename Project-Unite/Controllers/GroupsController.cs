using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
        public ActionResult CreateGroup()
        {
            //NOPE. I'm not circumming to the ways of Victor Tran. CURLY BRACES ON THEIR OWN LINE.
            var model = new GroupViewModel();
            return View(model);
        }

        private bool ValidateHex(string hex)
        {
            if (!(hex.Length == 3 || hex.Length == 6))
                return false;
            string hexallowed = "0123456789abcdef";
            foreach(var c in hex.ToLower().ToCharArray())
            {
                if (!hexallowed.Contains(c))
                    return false;
            }
            return true;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup(GroupViewModel model)
        {
            var result = ValidateHex(model.BannerColorHex);

            if(result == false)
            {
                ModelState.AddModelError("BannerColorHex", new Exception("Invalid hexadecimal color code."));
            }

            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();
            var group = new Group();
            group.Id = Guid.NewGuid().ToString();
            group.Name = model.Name;
            group.ShortName = model.ShortName;
            group.Description = model.Description;
            switch (model.Publicity)
            {
                case "public":
                    group.Publicity = 0;
                    break;
                case "publici":
                    group.Publicity = 1;
                    break;
                case "private":
                    group.Publicity = 2;
                    break;
                case "privatei":
                    group.Publicity = 3;
                    break;
            }
            group.RawReputation = 0.00;
            group.BannerColorHex = model.BannerColorHex;

            db.Groups.Add(group);
            db.SaveChanges();
            return RedirectToAction("JoinGroup", "Groups", new { id = group.Id });
        }

        [Authorize]
        public ActionResult JoinGroup(string id)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId());
            var group = db.Groups.FirstOrDefault(x => x.Id == id);
            if (group == null)
                return new HttpStatusCodeResult(404);
            user.GroupId = id;
            db.SaveChanges();
            return RedirectToAction("ViewGroup", "Groups", new { id = id });
        }

        [Authorize]
        public ActionResult LeaveGroup()
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId());
            var group = db.Groups.FirstOrDefault(x => x.Id == user.GroupId);
            if (group == null)
                return new HttpStatusCodeResult(404);
            user.GroupId = "";
            db.SaveChanges();
            return RedirectToAction("ViewGroup", "Groups", new { id = group.Id });

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