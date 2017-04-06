using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            return View(db.Downloads);
        }

        public ActionResult ViewRelease(string id)
        {
            var db = new ApplicationDbContext();
            var release = db.Downloads.FirstOrDefault(x => x.Id == id);
           
            return View(release);
        }
    }
}