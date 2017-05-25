using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Project_Unite.Models;

namespace Project_Unite
{
    public static class NotificationDaemon
    {
        public static Action<Notification> OnBroadcast;

        private static void SendMessage(string uid, string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<NotificationHub>().Clients.User(uid).sendMessage(message);
        }

        public static void NotifyFollowers(string uid, string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            if (user == null)
                throw new Exception("Cannot find user with ID " + uid + ".");
            foreach(var follower in user.Followers)
            {
                NotifyUser(uid, follower.Follower, title, desc, url);
            }
        }

        public static void NotifyEveryone(string uid, string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            if (user == null)
                throw new Exception("Cannot find user with ID " + uid + ".");
            foreach (var usr in db.Users.Where(x=>x.Id!=uid).ToArray())
            {
                NotifyUser(uid, usr.Id, title, desc, url);
            }
        }

        private static string ComposeHtml(Notification note)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<a href=\"" + note.ActionUrl + "\">");
            //Avatar holder start:
            builder.AppendLine("<div style=\"width:64px;height:64px;display:inline-block;\">");
            //Avatar
            builder.AppendLine("<img src=\"" + note.AvatarUrl + "\" width=\"64\" height=\"64\"/>");
            //Avatar holder end:
            builder.AppendLine("</div>");

            //Notification title.
            builder.AppendLine("<p><strong>" + note.Title + "</strong><br/><br/>");
            //Contents.
            builder.AppendLine(note.Description + "</p>");

            builder.AppendLine("</a>");
            return builder.ToString();

        }

    public static void NotifyUser(string uid, string target, string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            if (user == null)
                throw new Exception("Cannot find user with ID " + target + ".");
            string id = Guid.NewGuid().ToString();
            var note = new Notification
            {
                Id = id,
                UserId = target,
                Title = title,
                Timestamp = DateTime.Now,
                ActionUrl = $"http://getshiftos.ml/Manage/Notification/{id}?url={Uri.EscapeDataString(url)}",
                Description = desc,
                AvatarUrl = user.AvatarUrl
            };
            db.Notifications.Add(note);

            var t = db.Users.FirstOrDefault(x => x.Id == target);
            if (t.EmailOnNotifications)
            {
                if (t.LastLogin <= DateTime.Now.AddDays(-7))
                {
                    var man = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    man.SendEmailAsync(target, "New notification", $@"<h1>New notification</h1>

<h3>{note.Title}</h3>

<img src=""{note.AvatarUrl}"" width=""128"" height=""128"" style=""border-radius:100%""/>
<h4>{user.FullName}</h4>
<h5>{user.DisplayName}</h5>

<p>{note.Description}</p>

<a href=""{note.ActionUrl}"">Click here to acknowledge this notification.</a>");

                }
            }

            db.SaveChanges();

            SendMessage(target, ComposeHtml(note));
        }

        internal static void ScreamToDiscord(string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var conf = db.Configs.FirstOrDefault();
            if(conf != null)
            {
                if (!string.IsNullOrWhiteSpace(conf.WebhookUrl))
                {
                    var wc = HttpWebRequest.Create(conf.WebhookUrl);
                    wc.Method = "POST";
                    wc.ContentType = "application/json";
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        content = $@"**{title}**

{desc}

Visit this URL to see more: {url}"
                    });
                    wc.ContentLength = json.Length;
                    using (var s = wc.GetRequestStream())
                    {
                        using(var writer = new StreamWriter(s))
                        {
                            writer.Write(json);
                            using(var r = wc.GetResponse())
                            {
                                using(var rs = r.GetResponseStream())
                                {
                                    using(var reader = new StreamReader(rs))
                                    {
                                        string result = reader.ReadToEnd();
                                        db.AuditLogs.Add(new AuditLog("system", AuditLogLevel.Admin, "Discord webhook sent. Result: " + result));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}