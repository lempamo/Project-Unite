using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Discord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string query)
        {
            var result = new SearchResult();
            query = query.ToLower();
            var db = new ApplicationDbContext();


            result.Downloads = db.Downloads.Where(x => x.Name.ToLower().Contains(query) || x.Changelog.ToLower().Contains(query));
            result.ForumTopics = db.ForumTopics.Where(x => x.Subject.ToLower().Contains(query));
            result.Skins = db.Skins.Where(x => x.Name.ToLower().Contains(query) || x.ShortDescription.ToLower().Contains(query) || x.FullDescription.ToLower().Contains(query));
            result.Users = db.Users.Where(x => x.DisplayName.ToLower().Contains(query) || x.Bio.ToLower().Contains(query) || x.Interests.ToLower().Contains(query) || x.Hobbies.ToLower().Contains(query));
            result.WikiPages = db.WikiPages.Where(x => x.Name.ToLower().Contains(query) || x.Contents.ToLower().Contains(query));
            //Holy crap that search was... long.
            return View(result);


        }
    }
}