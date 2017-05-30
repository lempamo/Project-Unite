using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class PongHighscore
    {
        public string UserId { get; set; }
        public ulong CodepointsCashout { get; set; }
        public int Level { get; set; }
    }

    public class PongStatsViewModel
    {
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public List<PongHighscore> Highscores { get; set; }
    }
}