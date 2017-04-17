using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
            var note = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = target,
                Title = title,
                Timestamp = DateTime.Now,
                ActionUrl = url,
                Description = desc,
                AvatarUrl = user.AvatarUrl
            };
            db.Notifications.Add(note);

            db.SaveChanges();

            SendMessage(target, ComposeHtml(note));
        }


    }
}