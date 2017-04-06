using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

        public ActionResult AddRelease()
        {
            if (!ACL.Granted(User.Identity.Name, "CanReleaseBuilds"))
                return new HttpStatusCodeResult(403);
            ViewBag.Developer = true;

            var build = new PostDownloadViewModel();
            return View(build);
        }

        const string ApprovedIdChars = ".-_abcdefghijklmnopqrstuvwxyz1234567890";

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRelease(PostDownloadViewModel model)
        {
            if (!ACL.Granted(User.Identity.Name, "CanReleaseBuilds"))
                return new HttpStatusCodeResult(403);
            if (!ModelState.IsValid)
                return View(model);

            //Create a new database context.
            var db = new ApplicationDbContext();

            //Create a new "Download" data object
            var download = new Download();
            //Set the ID.
            string new_id = model.Name.ToLower();

            foreach(var c in new_id.ToCharArray())
            {
                if (!ApprovedIdChars.Contains(c))
                    new_id = new_id.Replace(c, '_');
            }
            new_id += "_" + db.Downloads.Count().ToString();
            download.Id = new_id;
            //Set the name.
            download.Name = model.Name;
            //Set the author
            download.ReleasedBy = User.Identity.GetUserId();
            //Set the time
            download.PostDate = DateTime.Now;

            //Can't forget the changelog. Literally, I almost forgot.
            download.Changelog = model.Changelog;

            //Set the YT ID
            download.DevUpdateId = model.DevUpdateId;

            //Set whether the build is stable
            download.IsStable = model.IsStable;

            //We're not obsolete.
            download.Obsolete = false;

            //Now we upload the download.

            string download_dir = "~/Uploads/Releases/";
            string mapped_dir = Server.MapPath(download_dir);
            if (!Directory.Exists(mapped_dir))
                Directory.CreateDirectory(mapped_dir);

            string file_name_d = model.Download.FileName;
            foreach(var c in file_name_d.ToCharArray())
            {
                if (!ApprovedIdChars.Contains(c))
                    file_name_d = file_name_d.Replace(c, '_');
            }
            download_dir += file_name_d;
            mapped_dir = Server.MapPath(download_dir);
            download.DownloadUrl = download_dir;
            //Now the download is saved in the DB. Let's get it on the server.
            model.Download.SaveAs(mapped_dir);

            download_dir = "~/Uploads/Releases/Screenshots/";
            mapped_dir = Server.MapPath(download_dir);
            if (!Directory.Exists(mapped_dir))
                Directory.CreateDirectory(mapped_dir);

            file_name_d = model.Screenshot.FileName;
            foreach (var c in file_name_d.ToCharArray())
            {
                if (!ApprovedIdChars.Contains(c))
                    file_name_d = file_name_d.Replace(c, '_');
            }
            download_dir += file_name_d;
            mapped_dir = Server.MapPath(download_dir);
            download.ScreenshotUrl = download_dir;
            model.Screenshot.SaveAs(mapped_dir);

            //Now we just save to the database...
            db.Downloads.Add(download);
            db.SaveChanges();

            return RedirectToAction("Releases");
        }
    }


}