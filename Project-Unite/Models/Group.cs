using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Publicity { get; set; }
        public string BannerColorHex { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }

        public double RawReputation { get; set; }

        public int Reputation
        {
            get
            {
                return ((int)Math.Round(RawReputation));
            }
        }

    }
}