using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class APIController : Controller
    {
        public ActionResult GetPongCP()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Content(user.Pong_HighestCodepointsCashout.ToString());
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult SetPongCP(int id)
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);

                var db = new ApplicationDbContext();
                var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
                var user = db.Users.FirstOrDefault(x => x.Id == t.UserId);
                user.Pong_HighestCodepointsCashout = id;
                db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        public ActionResult GetPongLevel()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Content(user.Pong_HighestLevel.ToString());
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult SetPongLevel(int id)
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);

                var db = new ApplicationDbContext();
                var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
                var user = db.Users.FirstOrDefault(x => x.Id == t.UserId);

                user.Pong_HighestLevel = id;
                db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }




        public ActionResult GetCodepoints()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Content(user.Codepoints.ToString());
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult SetCodepoints(long id)
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);

                var db = new ApplicationDbContext();
                var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
                var user = db.Users.FirstOrDefault(x => x.Id == t.UserId);
                user.Codepoints = id;
                db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        public ActionResult GetEmail()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Content(user.Email);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult GetSysName()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Content(user.SystemName);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult SetSysName(string id)
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);

                var db = new ApplicationDbContext();
                var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
                var user = db.Users.FirstOrDefault(x => x.Id == t.UserId);
                user.SystemName = id;
                db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult GetPongHighscores()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                var db = new ApplicationDbContext();
                var highscores = new List<PongHighscore>();
                foreach (var u in db.Users)
                {
                    highscores.Add(new PongHighscore
                    {
                        UserId = u.Id,
                        Level = u.Pong_HighestLevel,
                        CodepointsCashout = u.Pong_HighestCodepointsCashout
                    });
                }

                int pagecount = highscores.GetPageCount(10);
                var pages = highscores.OrderByDescending(x => x.Level).ToArray();

                var model = new PongStatsViewModel
                {
                    Highscores = pages.ToList(),
                    CurrentPage = 0,
                    PageCount = 10
                };

                return Content(Serializer.Serialize(model));
            }
            catch
            {
                return new HttpStatusCodeResult(403);
            }
        }


        public ActionResult GetDisplayName(string id = "")
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                if(string.IsNullOrWhiteSpace(id))
                    return Content(user.DisplayName);

                var db = new ApplicationDbContext();
                var uwithid = db.Users.FirstOrDefault(x => x.Id == id);
                if (uwithid == null)
                    return new HttpStatusCodeResult(404);
                return Content(uwithid.DisplayName);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult SetDisplayName(string id)
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);

                var db = new ApplicationDbContext();
                var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
                var user = db.Users.FirstOrDefault(x => x.Id == t.UserId);
                user.DisplayName = id;
                db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult GetFullName()
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);
                var user = ACL.GetUserFromToken(token);
                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Content(user.FullName);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult SetFullName(string id)
        {
            try
            {
                string token = Request.Headers["Authentication"].Remove(0, 6);

                var db = new ApplicationDbContext();
                var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
                var user = db.Users.FirstOrDefault(x => x.Id == t.UserId);
                user.FullName = id;
                db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }



        public JavaScriptSerializer Serializer
        {
            get; set;
        }
        //I'll put those curly braces on their own line just to piss a certain smiley off.

        public APIController()
        {
            Serializer = new JavaScriptSerializer();
        }

        // GET: API
        public ActionResult Index()
        {
            return Content("Please use a valid API endpoint.");
        }

        public ActionResult Releases(bool showUnstable = false, bool showObsolete = false)
        {
            var db = new ApplicationDbContext();

            var releases = db.Downloads.ToArray();
            if (!showUnstable)
                releases = releases.Where(x => x.IsStable == true).ToArray();
            if (!showObsolete)
                releases = releases.Where(x => x.Obsolete == false).ToArray();

            return Content(Serializer.Serialize(releases));
        }

        public ActionResult Skins()
        {
            var db = new ApplicationDbContext();
            return Content(Serializer.Serialize(db.Skins.ToArray()));
        }

        public ActionResult TestNotification()
        {
            NotificationDaemon.NotifyEveryone(User.Identity.GetUserId(), "Test notification", "This is a test of the real-time notification system.", "#");
            return Content("Sent.");
        }

        [Authorize]
        public ActionResult GetNotificationCount()
        {
            return Content(ACL.NotificationCountRaw(User.Identity.Name).ToString());
        }
    }
}