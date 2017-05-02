using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class StatsController : Controller
    {
        // GET: Stats
        public ActionResult Pong(int id = 1)
        {
            var db = new ApplicationDbContext();
            var highscores = new List<PongHighscore>();
            foreach(var user in db.Users)
            {
                highscores.Add(new PongHighscore
                {
                    UserId = user.Id,
                    Level = user.Pong_HighestLevel,
                    CodepointsCashout = user.Pong_HighestCodepointsCashout
                });
            }

            id = id - 1;

            int pagecount = highscores.GetPageCount(10);
            if (id > pagecount || id < 1)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var pages = highscores.OrderByDescending(x=>x.Level).ToArray().GetItemsOnPage(id, 10);

            var model = new PongStatsViewModel
            {
                Highscores = pages.ToList(),
                CurrentPage = id,
                PageCount = 10
            };

            return View(model);
        }
    }
}