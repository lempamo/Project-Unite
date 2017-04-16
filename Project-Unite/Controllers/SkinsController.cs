﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class SkinsController : Controller
    {
        // GET: Skins
        public ActionResult Index()
        {
            return View(new ApplicationDbContext().Skins);
        }

        public ActionResult ViewSkin(string id)
        {
            var db = new ApplicationDbContext();
            var skn = db.Skins.FirstOrDefault(x => x.Id == id);
            if (skn == null)
                return new HttpStatusCodeResult(404);
            return View(skn);
        }

        [Authorize]
        public ActionResult PostSkin()
        {
            var model = new CreateSkinViewModel();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostSkin(CreateSkinViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var db = new ApplicationDbContext();
            var skin = new Skin();

            string allowed = "abcdefghijklmnopqrstuvwxyz1234567890-_";

            string id = model.Title.ToLower();
            foreach(char c in id.ToCharArray())
            {
                if (!allowed.Contains(c))
                    id = id.Replace(c, '_');
            }

            skin.Id = id + "_" + db.Skins.Count().ToString();
            skin.Name = model.Title;
            skin.ShortDescription = model.ShortDescription;
            skin.PostedAt = DateTime.Now;
            skin.FullDescription = model.LongDescription;
            skin.UserId = User.Identity.GetUserId();
            skin.VersionId = "";
            string repoFolder = $"~/Uploads/{ACL.UserNameRaw(skin.UserId)}/SkinFiles";
            string screenshotFolder = $"~/Uploads/{ACL.UserNameRaw(skin.UserId)}/Screenshots";
            if (!Directory.Exists(Server.MapPath(repoFolder)))
                Directory.CreateDirectory(Server.MapPath(repoFolder));
            if (!Directory.Exists(Server.MapPath(screenshotFolder)))
                Directory.CreateDirectory(Server.MapPath(screenshotFolder));


            skin.DownloadUrl = repoFolder.Remove(0,1) + "/" + model.SkinFile.FileName;
            model.SkinFile.SaveAs(Path.Combine(Server.MapPath(repoFolder), model.SkinFile.FileName));

            if (model.ScreenshotFile != null && model.ScreenshotFile.ContentLength > 0)
            {
                skin.ScreenshotUrl = screenshotFolder.Remove(0, 1) + "/" + model.ScreenshotFile.FileName;
                model.ScreenshotFile.SaveAs(Path.Combine(Server.MapPath(screenshotFolder), model.ScreenshotFile.FileName));
            }
            db.Skins.Add(skin);
            db.SaveChanges();
            return RedirectToAction("ViewSkin", new { id = skin.Id });
        }
    }
}