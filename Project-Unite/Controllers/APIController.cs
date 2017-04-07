using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class APIController : Controller
    {
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

            return Content(JsonConvert.SerializeObject(releases));
        }

        public ActionResult Skins()
        {
            var db = new ApplicationDbContext();
            return Content(JsonConvert.SerializeObject(db.Skins.ToArray()));
        }
    }
}