using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Controllers
{
    public class LegalController : Controller
    {
        // GET: Legal/TOS
        public ActionResult TOS()
        {
            return View();
        }

        public ActionResult License()
        {
            return View();
        }

        // GET: Legal/Privacy
        public ActionResult Privacy()
        {
            return View();
        }
    }
}