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
    [RequiresDeveloper]
    [Authorize]
    public class DeveloperController : Controller
    {
        // GET: Developer
        public ActionResult Index(string id = "home")
        {
            ViewBag.Page = id;
            return View();
        }

        public ActionResult ToggleObsolete(string id)
        {
            var db = new ApplicationDbContext();
            var release = db.Downloads.FirstOrDefault(x => x.Id == id);
            release.Obsolete = !release.Obsolete;
            db.SaveChanges();
            return RedirectToAction("Releases");
        }

        public ActionResult MakeUnstable(string id)
        {
            var db = new ApplicationDbContext();
            var release = db.Downloads.FirstOrDefault(x => x.Id == id);
            release.IsStable = false;
            db.SaveChanges();
            return RedirectToAction("Releases");
        }


        public ActionResult MakeStable(string id)
        {
            var db = new ApplicationDbContext();
            var release = db.Downloads.FirstOrDefault(x => x.Id == id);
            release.IsStable = true;
            db.SaveChanges();
            return RedirectToAction("Releases");
        }


        public ActionResult Releases()
        {
            return RedirectToAction("Index", new { id = "releases" });
        }

        public ActionResult AddRelease()
        {
            ViewBag.Developer = true;

            var build = new PostDownloadViewModel();
            return View(build);
        }

        const string ApprovedIdChars = ".-_abcdefghijklmnopqrstuvwxyz1234567890";

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRelease(PostDownloadViewModel model)
        {
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
                if (!ApprovedIdChars.Contains(c) || c == '.')
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

            string file_name_d = model.Download.FileName.ToLower();
            foreach(var c in file_name_d.ToCharArray())
            {
                if (!ApprovedIdChars.Contains(c))
                    file_name_d = file_name_d.Replace(c, '_');
            }
            download_dir += file_name_d;
            mapped_dir = Server.MapPath(download_dir);
            download.DownloadUrl = download_dir.Remove(0, 1);
            //Now the download is saved in the DB. Let's get it on the server.
            model.Download.SaveAs(mapped_dir);

            if (model.Screenshot != null)
            {
                download_dir = "~/Uploads/Releases/Screenshots/";
                mapped_dir = Server.MapPath(download_dir);
                if (!Directory.Exists(mapped_dir))
                    Directory.CreateDirectory(mapped_dir);

                file_name_d = model.Screenshot.FileName.ToLower(); ;
                foreach (var c in file_name_d.ToCharArray())
                {
                    if (!ApprovedIdChars.Contains(c))
                        file_name_d = file_name_d.Replace(c, '_');
                }
                download_dir += file_name_d;
                mapped_dir = Server.MapPath(download_dir);
                download.ScreenshotUrl = download_dir.Remove(0, 1);
                model.Screenshot.SaveAs(mapped_dir);
            }

            string downloadIsStable = (download.IsStable) ? "Yes" : "No";

            //Now we just save to the database...
            db.Downloads.Add(download);

            NotificationDaemon.ScreamToDiscord("New release: " + download.Name, $@"A new release of ShiftOS has been made!

Release name: {download.Name}
Stable: {downloadIsStable}
Released on: {download.PostDate}", Url.Action("ViewRelease", "Downloads", new { id = download.Id }));

            db.SaveChanges();

            return RedirectToAction("Releases");
        }
        
        [Authorize]
        public ActionResult Wiki()
        {
            return RedirectToAction("Index", new { id = "wiki" });
        }

        public ActionResult AddWikiCategory()
        {
            ViewBag.Developer = true;

            var mdl = new AddWikiCategoryViewModel();
            return View(mdl);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddWikiCategory(AddWikiCategoryViewModel model)
        {
            ViewBag.Developer = true;
            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();
            var cat = new WikiCategory();

            var new_id = model.Name.ToLower();

            foreach(var c in new_id.ToArray())
            {
                if(c == '.' || !ApprovedIdChars.Contains(c))
                {
                    new_id = new_id.Replace(c, '_');
                }
            }

            cat.Id = new_id;
            cat.Name = model.Name;

            cat.Parent = model.ParentId;

            db.WikiCategories.Add(cat);
            db.SaveChanges();

            return RedirectToAction("Wiki");



        }

    }


}