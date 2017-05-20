using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class ContestsController : Controller
    {
        // GET: Contests
        public ActionResult Index()
        {
            var model = new ApplicationDbContext().Contests.ToArray();
            return View(model);
        }
    }
}