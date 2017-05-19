using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult SendFeedback()
        {
            var sfm = new SendFeedbackViewModel();
            if(Request.IsAuthenticated)
            {
                var db = new ApplicationDbContext();
                var user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                sfm.Name = (string.IsNullOrWhiteSpace(user.FullName)) ? user.DisplayName : user.FullName;
                sfm.Email = user.Email;
            }
            return View(sfm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFeedback(SendFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();
            var siteconfig = db.Configs.FirstOrDefault();

            var mailsender = new EmailService();
            var message = new IdentityMessage
            {
                Destination = siteconfig.FeedbackEmail,
                Subject = "[Feedback] " + model.Name,
                Body = $@"<h1>Project: Unite Feedback</h1>

<dl>
    <dt>From:</dt>
    <dd>{model.Name} [{model.Email}]</dd>
    <dt>Type:</dt>
    <dd>{model.FeedbackType}</dd>
</dl>

<hr/>

{ACL.MarkdownRaw(model.Body)}"
            };
            mailsender.SendAsync(message);
            return RedirectToAction("Index");
        }

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