using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class Download
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [AllowHtml]
        public string Changelog { get; set; }

        [Required]
        public string DownloadUrl { get; set; }

        [Required]
        public bool Obsolete { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        [Required]
        public string ReleasedBy { get; set; }

        public string DevUpdateId { get; set; }


        public string ScreenshotUrl { get; set; }

        [Required]
        public bool IsStable { get; set; }
    }

    public class PostDownloadViewModel
    {
        [Required]
        public string Name { get; set; }

        public HttpPostedFileBase Screenshot { get; set; }

        public string DevUpdateId { get; set; }

        [Required]
        public bool IsStable { get; set; }

        [Required]
        [AllowHtml]
        public string Changelog { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Download { get; set; }
        
    }
}