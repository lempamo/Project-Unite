using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class APIController : Controller
    {
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

        [Authorize]
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