using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class WikiController : Controller
    {
        public ActionResult Index(string id = "")
        {
            var db = new ApplicationDbContext();
            var model = new WikiViewModel();
            var wikicategories = db.WikiCategories.Where(x => x.Parent == "none");
            model.Categories = wikicategories;
            model.Page = db.WikiPages.FirstOrDefault(x => x.Id == id);
            return View(model);
        }
    }
}